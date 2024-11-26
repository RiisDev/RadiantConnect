using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RadiantConnect.Authentication.DriverRiotAuth.Records;

/*
 *
 * Huge credit to https://github.com/judongdev/riot-qr-auth/blob/main/api/index.py for implementation <3
 *
 */

namespace RadiantConnect.Authentication.QRSignIn
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
            UrlBuilder builder = new(HttpClient);

            BuiltData loginUrl = await builder.Build(code);

            Debug.WriteLine($"https://api.qrserver.com/v1/create-qr-code/?size=512x512&data={loginUrl.LoginUrl}");

            return null;
        }

        public void Dispose() => HttpClient.Dispose();
    }
}
