using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
    internal class SignInManager : IDisposable
    {
        internal static CookieContainer Container = new();
        internal static HttpClient HttpClient = new(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
            UseCookies = true,
            CookieContainer = Container,
            AutomaticDecompression = DecompressionMethods.All
        });

        internal static async Task<RSOAuth?> InitiateSignIn(Authentication.CountryCode code)
        {
            LoginQrManager builder = new(HttpClient);
            BuiltData qrData = await builder.Build(code);

            string urlProper = HttpUtility.UrlEncode(qrData.LoginUrl);
            byte[] imageData = await HttpClient.GetByteArrayAsync($"https://api.qrserver.com/v1/create-qr-code/?size=250x250&data={urlProper}");

            MemoryStream stream = new(imageData);
            Bitmap bitmap = new(stream);
            Win32Form form = new(bitmap);
            TokenManager manager = new(form, qrData, HttpClient);

            manager.InitiateTimer();

            return null;
        }

        public void Dispose() => HttpClient.Dispose();
    }
}
