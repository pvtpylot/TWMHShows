using System.Diagnostics;
using System.Text;

namespace MauiBlazorWeb.Services;

/// <summary>
///     Default implementation of INetworkDiagnostics that performs basic connectivity tests.
/// </summary>
public class DefaultNetworkDiagnostics : INetworkDiagnostics
{
    public async Task<bool> PerformConnectivityTestAsync()
    {
        try
        {
            Debug.WriteLine("Performing diagnostic connection test...");
            var httpClient = GetHttpClient();
            Debug.WriteLine($"Testing connection to: {HttpClientHelper.BaseUrl}");

            // Try a simple GET request to the base URL
            var response = await httpClient.GetAsync(HttpClientHelper.BaseUrl);
            Debug.WriteLine($"Diagnostic test response: {(int)response.StatusCode} {response.StatusCode}");

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Diagnostic test failed: {ex.Message}");
            if (ex.InnerException != null) Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
            return false;
        }
    }

    public async Task<string> GenerateDiagnosticReportAsync()
    {
        var results = new StringBuilder();

        try
        {
            // Get platform info
            results.AppendLine($"Platform: {DeviceInfo.Platform}");
            results.AppendLine($"Device type: {DeviceInfo.Idiom}");
            results.AppendLine($"OS version: {DeviceInfo.VersionString}");

            // Test connectivity to our backend
            var baseUrlConnectivity = await PerformConnectivityTestAsync();
            results.AppendLine($"Backend connectivity: {(baseUrlConnectivity ? "OK" : "Failed")}");

            // Try internet connectivity
            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync("https://httpbin.org/get");
                results.AppendLine($"Internet connectivity: {(response.IsSuccessStatusCode ? "OK" : "Failed")}");
            }
            catch (Exception ex)
            {
                results.AppendLine($"Internet connectivity error: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            results.AppendLine($"Error generating diagnostic report: {ex.Message}");
        }

        return results.ToString();
    }

    protected virtual HttpClient GetHttpClient()
    {
        return HttpClientHelper.GetHttpClient();
    }
}