internal class HttpClientHelper
{
    //TODO: Place this in AppSettings or Client config file
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
                // For iOS simulator, use 127.0.0.1 explicitly instead of localhost
                _baseUrl = "https://127.0.0.1:7157/";
            }
#endif
            return _baseUrl;
        }
    }
    
    public static string LoginUrl => $"{BaseUrl}identity/login";
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

    private static HttpMessageHandler GetPlatformHandler()
    {
#if ANDROID
        var handler = new Xamarin.Android.Net.AndroidMessageHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        return handler;
#elif IOS
        // Use HttpClientHandler instead of NSUrlSessionHandler for better compatibility
        var handler = new HttpClientHandler
        {
            // Always trust the certificate for development
            ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => true
        };
        return handler;
#else
        throw new PlatformNotSupportedException("Only Android and iOS supported.");
#endif
    }
}