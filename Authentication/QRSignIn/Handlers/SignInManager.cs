using System.Web;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Authentication.QRSignIn.Modules;

/*
 *
 * Huge credit to https://github.com/judongdev/riot-qr-auth/blob/main/api/index.py for implementation <3
 *
 */

namespace RadiantConnect.Authentication.QRSignIn.Handlers
{
    public delegate void UrlBuilder(string url);

    internal class SignInManager(Authentication.CountryCode code, bool returnUrl = false)
    {
        internal event UrlBuilder? OnUrlBuilt;

        internal Process DisplayImage(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("QR code image not found", path);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = path,
                    UseShellExecute = true
                };
                return Process.Start(startInfo)!;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP") ?? Environment.GetEnvironmentVariable("DESKTOP_SESSION")))
                    throw new InvalidOperationException("No desktop environment detected, please use ReturnUrl.");
                return Process.Start("xdg-open", path);
            }

            throw new PlatformNotSupportedException("Unsupported OS");
        }

        internal async Task<RSOAuth?> Authenticate()
        {
            (HttpClient httpClient, CookieContainer container) = AuthUtil.BuildClient();

            LoginQrManager builder = new(httpClient);
            BuiltData qrData = await builder.Build(code);

            string tempName = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.jpg");
            Process? form = null;
            
            try
            {
                if (returnUrl)
                {
                    OnUrlBuilt?.Invoke(qrData.LoginUrl);
                }
                else
                {
                    string urlProper = HttpUtility.UrlEncode(qrData.LoginUrl);
                    byte[] imageData = await httpClient.GetByteArrayAsync($"https://api.qrserver.com/v1/create-qr-code/?size=250x250&data={urlProper}");
                    await File.WriteAllBytesAsync(tempName, imageData);
                    form = DisplayImage(tempName);
                }
                
                TokenManager manager = new(form, qrData, httpClient, returnUrl, container);
                TaskCompletionSource<RSOAuth?> tcs = new();

                manager.OnTokensFinished += authData => tcs.SetResult(authData);
                manager.InitiateTimer(tempName);

                return await tcs.Task;
            }
            finally
            {
                httpClient.Dispose();
                try { form?.Kill(true); }catch{/**/}
                try { Process.GetProcessesByName(tempName).ToList().ForEach(x => x.Kill(true)); } catch {/**/}

                form?.Dispose();

                if (File.Exists(tempName)) File.Delete(tempName);
            }
        }
    }
}

