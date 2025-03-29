#if WINDOWS
using System.Drawing;
#endif
using System.Net;
using System.Web;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Authentication.QRSignIn.Modules;
using RadiantConnect.Utilities;

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

        internal async Task<RSOAuth?> Authenticate()
        {
            (HttpClient httpClient, CookieContainer container) = AuthUtil.BuildClient();

            LoginQrManager builder = new(httpClient);
            BuiltData qrData = await builder.Build(code);
#if WINDOWS
            MemoryStream? stream = null;
            Win32Form? form = null;
            Bitmap? bitmap = null;
#endif
            try
            {
                if (returnUrl)
                {
                    OnUrlBuilt?.Invoke(qrData.LoginUrl);
                }
                else
                {
#if WINDOWS
                    string urlProper = HttpUtility.UrlEncode(qrData.LoginUrl);
                    byte[] imageData = await httpClient.GetByteArrayAsync($"https://api.qrserver.com/v1/create-qr-code/?size=250x250&data={urlProper}");

                    stream = new MemoryStream(imageData);
                    bitmap = new Bitmap(stream);
                    form = new Win32Form(bitmap);
#else
                    throw new RadiantConnectAuthException("QR Code generation cannot be displayed on non-windows applications, please hook `OnUrlBuilt`");
#endif
                }
#if WINDOWS
                TokenManager manager = new(form, qrData, httpClient, returnUrl, container);
#else
                TokenManager manager = new(null, qrData, httpClient, returnUrl, container);
#endif
                TaskCompletionSource<RSOAuth?> tcs = new();

                manager.OnTokensFinished += authData => tcs.SetResult(authData);
                manager.InitiateTimer();

                return await tcs.Task;
            }
            finally
            {
                httpClient.Dispose();
#if WINDOWS
                form?.Dispose();
                bitmap?.Dispose();
                stream?.Dispose();
#endif
            }
        }
    }
}

