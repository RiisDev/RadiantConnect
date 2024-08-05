using Microsoft.IdentityModel.JsonWebTokens;
using RadiantConnect.Authentication.DriverRiotAuth;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace RadiantConnect.Authentication
{
    public class Authentication
    {
        public enum DriverStatus {
            Checking_Existing_Processes,
            Creating_Driver,
            Redirecting_To_RSO,
            Driver_Created,
            Begin_SignIn,
            Clearing_Cached_Auth,
            Checking_RSO_Login_Page,
            Logging_Into_RSO,
            Logging_Into_Valorant,
            Checking_RSO_Multi_Factor,
            Grabbing_Required_Tokens,
            Multi_Factor_Requested,
            Multi_Factor_Completed,
            SignIn_Completed,
            Cookies_Received
        }

        public enum CaptchaService
        {
            SuperMemory
        }
        
        public record DriverSettings(
            string ProcessName = "msedge",
            string BrowserExecutable = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
            bool KillBrowser = false
        );

        public record RSOAuth(
            string? Subject,
            string? SSID,
            string? TDID,
            string? CSID,
            string? CLID,
            string? PVPNetToken,
            string? IdToken,
            string? AccessToken,
            string? PasToken,
            string? Entitlement, 
            string? Affinity,
            string? ChatAffinity,
            string? ClientConfig
        );

        public delegate void MultiFactorEvent();
        public event MultiFactorEvent? OnMultiFactorRequested;

        public delegate void DriverEvent(DriverStatus status);
        public event DriverEvent? OnDriverUpdate;

        public string? MultiFactorCode
        {
            get => authHandler.MultiFactorCode;
            set => authHandler.MultiFactorCode = value;
        }

        internal AuthHandler authHandler = null!;

        public async Task<RSOAuth?> AuthenticateWithCaptcha(string username, string password, CaptchaService service, string captchaAuthorization)
        {
            await Task.Delay(1);
            return null;
        }

        public async Task<RSOAuth?> AuthenticateWithDriver(string username, string password, DriverSettings? driverSettings = null)
        {   
            driverSettings ??= new DriverSettings();

            if (driverSettings.ProcessName == "chrome")
                throw new RadiantConnectAuthException(
                    "Chrome is unsupported, please use another chromium browser (edge, brave)");

            authHandler = new AuthHandler(driverSettings.ProcessName, driverSettings.BrowserExecutable, driverSettings.KillBrowser);

            authHandler.OnMultiFactorRequested += () => OnMultiFactorRequested?.Invoke();
            authHandler.OnDriverUpdate += status => OnDriverUpdate?.Invoke(status);

            (IEnumerable<Cookie>? cookies, string? accessToken, string? pasToken, string? entitlement, string? clientConfig) = await authHandler.Initialize(username, password);

            if (cookies == null) return null;
            
            IEnumerable<Cookie> enumerable = cookies as Cookie[] ?? cookies.ToArray();
            string? rsoSubject = enumerable.FirstOrDefault(x => x.Name == "sub")?.Value;
            string? rsoSsid = enumerable.FirstOrDefault(x => x.Name == "ssid")?.Value;
            string? rsoTdid = enumerable.FirstOrDefault(x => x.Name == "tdid")?.Value;
            string? rsoCsid = enumerable.FirstOrDefault(x => x.Name == "csid")?.Value;
            string? rsoClid = enumerable.FirstOrDefault(x => x.Name == "clid")?.Value;
            string? pvpNet = enumerable.FirstOrDefault(x => x.Name == "PVPNET_TOKEN_NA")?.Value;
            string? idToken = enumerable.FirstOrDefault(x => x.Name == "id_token")?.Value;

            JsonWebToken jwt = new(pasToken);
            string? affinity = jwt.GetPayloadValue<string>("affinity");
            string? chatAffinity = jwt.GetPayloadValue<string>("desired.affinity");
            
            return new RSOAuth(rsoSubject, rsoSsid, rsoTdid, rsoCsid, rsoClid, pvpNet, idToken, accessToken, pasToken, entitlement, affinity, chatAffinity, clientConfig);
        }

        public async Task Logout() => await authHandler.Logout();
    }
}
