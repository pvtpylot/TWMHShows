using System.Text;

internal class HttpClientHelper
{
    private static string _baseUrl = "https://localhost:7157/";
    public static string BaseUrl
    {
        get
        {
#if DEBUG
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                _baseUrl = _baseUrl.Replace("localhost", "10.0.2.2");
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                bool isSimulator = DeviceInfo.DeviceType == DeviceType.Virtual;
                if (isSimulator)
                {
                    // Use the correct port for your server
                    _baseUrl = "https://10.0.0.246:7157/";
                }
                else
                {
                    // Physical device - use the IP address and correct port
                    _baseUrl = "https://10.0.0.246:7157/";
                }
            }
#endif
            return _baseUrl;
        }
    }
    
    public static string LoginUrl => $"{BaseUrl}identity/mobilelogin";
    public static string RefreshUrl => $"{BaseUrl}identity/refresh";
    public static string WeatherUrl => $"{BaseUrl}api/weather";

    public static HttpClient GetHttpClient()
    {
#if WINDOWS || MACCATALYST
        return new HttpClient();
#else
        return new HttpClient(GetPlatformHandler());
#endif
    }

    public static HttpMessageHandler GetPlatformHandler()
    {
#if ANDROID
        var handler = new Xamarin.Android.Net.AndroidMessageHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        return handler;
#elif IOS
        // Enhanced handler for iOS with better logging
        var handler = new HttpClientHandler
        {
            // Always trust the certificate for development
            ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => {
                Console.WriteLine($"iOS SSL validation bypassed. Cert: {cert?.Subject}, Errors: {errors}");
                return true;
            },
            AllowAutoRedirect = true
        };
        return handler;
#else
        throw new PlatformNotSupportedException("Only Android and iOS supported.");
#endif
    }

    public static async Task<string> TestServerConnectivity()
    {
        var result = new StringBuilder();
        
        try
        {
            result.AppendLine($"Testing connectivity to {BaseUrl}");
            result.AppendLine($"Platform: {DeviceInfo.Platform}, DeviceType: {DeviceInfo.DeviceType}");
            
            var handler = GetPlatformHandler();
            using var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(10);
            
            // First try a HEAD request which is lightweight
            using var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, BaseUrl));
            result.AppendLine($"Server responded: {(int)response.StatusCode} {response.StatusCode}");
            
            return result.ToString();
        }
        catch (Exception ex)
        {
            result.AppendLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
                result.AppendLine($"Inner Error: {ex.InnerException.Message}");
            
            return result.ToString();
        }
    }
}