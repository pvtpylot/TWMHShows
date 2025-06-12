using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MauiBlazorWeb.Services
{
    public static class NetworkDiagnostics
    {
        public static async Task<string> RunNetworkDiagnosticsAsync()
        {
            var results = new System.Text.StringBuilder();
            try
            {
                // Get platform info
                results.AppendLine($"Platform: {DeviceInfo.Platform}");
                results.AppendLine($"Device type: {DeviceInfo.Idiom}");
                
                // Test plain HTTP client
                try
                {
                    var httpClient = new HttpClient(new HttpClientHandler 
                    { 
                        ServerCertificateCustomValidationCallback = (m, c, ch, e) => true 
                    });
                    
                    // Try to connect to a public endpoint
                    var response = await httpClient.GetAsync("https://httpbin.org/get");
                    results.AppendLine($"Public endpoint: {response.StatusCode}");
                    
                    // Try to connect to our server
                    response = await httpClient.GetAsync(HttpClientHelper.BaseUrl);
                    results.AppendLine($"Server connection: {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    results.AppendLine($"HTTP client error: {ex.Message}");
                }
                
                // Try TCP connection
                try
                {
                    var uri = new Uri(HttpClientHelper.BaseUrl);
                    using var tcpClient = new TcpClient();
                    await tcpClient.ConnectAsync(uri.Host, uri.Port);
                    results.AppendLine($"TCP connection to {uri.Host}:{uri.Port}: Success");
                }
                catch (Exception ex)
                {
                    results.AppendLine($"TCP connection error: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                results.AppendLine($"Diagnostic error: {ex}");
            }
            
            Debug.WriteLine(results.ToString());
            return results.ToString();
        }
    }
}