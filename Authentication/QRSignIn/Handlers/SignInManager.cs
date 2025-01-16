using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Text.Json;
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

    internal class SignInManager(Authentication.CountryCode code, bool returnUrl = false)// : IDisposable
    {
        internal event UrlBuilder? OnUrlBuilt;
        
        internal async Task<RSOAuth?> InitiateSignIn()
        {
            (HttpClient HttpClient, CookieContainer Container) = Util.BuildClient();

            LoginQrManager builder = new(HttpClient);
            BuiltData qrData = await builder.Build(code);
            Win32Form? form = null;

            if (returnUrl)
            {
                OnUrlBuilt?.Invoke(qrData.LoginUrl);
            }
            else
            {
                string urlProper = HttpUtility.UrlEncode(qrData.LoginUrl);
                byte[] imageData =
                    await HttpClient.GetByteArrayAsync(
                        $"https://api.qrserver.com/v1/create-qr-code/?size=250x250&data={urlProper}");

                MemoryStream stream = new(imageData);
                Bitmap bitmap = new(stream);
                form = new Win32Form(bitmap);
            }

            TokenManager manager = new(form, qrData, HttpClient, returnUrl, Container);

            TaskCompletionSource<RSOAuth?> tcs = new ();

            manager.OnTokensFinished += authData => tcs.SetResult(authData);

            manager.InitiateTimer();

            return await tcs.Task;
        }

        //public void Dispose() => HttpClient.Dispose();
    }
}
