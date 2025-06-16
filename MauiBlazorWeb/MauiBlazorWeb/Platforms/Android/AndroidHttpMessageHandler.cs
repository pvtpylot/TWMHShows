using System.Net.Http;
using Android.Net;
using Javax.Net.Ssl;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace MauiBlazorWeb.Services
{
    public class AndroidHttpMessageHandler : HttpClientHandler
    {
        public AndroidHttpMessageHandler()
        {
            // Trust all certificates for development only
            ServerCertificateCustomValidationCallback = (message, certificate, chain, errors) => 
            {
                Debug.WriteLine($"Android certificate validation callback - certificate: {certificate?.Subject}, errors: {errors}");
                return true;
            };
        }
    }
}