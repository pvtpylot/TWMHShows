using System.Diagnostics;

namespace MauiBlazorWeb.Services;

public static class HttpClientHelper
{
    public static string BaseUrl => GetBaseUrl();
    public static string LoginUrl => $"{BaseUrl}/identity/mobilelogin";
    public static string RefreshUrl => $"{BaseUrl}/identity/refresh";

    public static string WeatherUrl => $"{BaseUrl}/api/weather";

    // Use local IP for the emulator to access the host machine
    private static string GetBaseUrl()
    {
        // Special case for Android - use 10.0.2.2 for localhost
        if (DeviceInfo.Platform == DevicePlatform.Android) return "https://10.0.2.2:7157";

        // For iOS simulator or physical devices
        return "https://localhost:7157";
    }

    public static HttpClient GetHttpClient()
    {
#if ANDROID
            // Android specific handler with certificate validation disabled for development
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            var httpClient = new HttpClient(handler);
            Debug.WriteLine($"Created Android HttpClient with custom handler for: {BaseUrl}");
#elif IOS
            // iOS specific handler with certificate validation disabled for development
            var handler = new NSUrlSessionHandler
            {
                TrustOverrideForUrl = (_, _, _) => true
            };
            var httpClient = new HttpClient(handler);
            Debug.WriteLine($"Created iOS HttpClient with custom handler for: {BaseUrl}");
#else
        // Default handler for other platforms
        var httpClient = new HttpClient();
        Debug.WriteLine($"Created default HttpClient for: {BaseUrl}");
#endif

        httpClient.Timeout = TimeSpan.FromSeconds(30);

        // Clear any previous headers
        httpClient.DefaultRequestHeaders.Clear();

        // Add common headers
        httpClient.DefaultRequestHeaders.Add("User-Agent", "MauiBlazorWebApp");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        return httpClient;
    }
}