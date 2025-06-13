using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
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
        
        public static async Task<string> TestLoginEndpoint()
        {
            var results = new StringBuilder();
            try
            {
                results.AppendLine($"Testing login endpoint: {HttpClientHelper.LoginUrl}");
            
                // Create a handler with certificate validation disabled
                var handler = HttpClientHelper.GetPlatformHandler();
                using var client = new HttpClient(handler);
            
                // Try a simple OPTIONS request to check CORS
                var optionsMsg = new HttpRequestMessage(HttpMethod.Options, HttpClientHelper.LoginUrl);
                try
                {
                    var optionsResponse = await client.SendAsync(optionsMsg);
                    results.AppendLine($"OPTIONS response: {(int)optionsResponse.StatusCode} {optionsResponse.StatusCode}");
                
                    // List all headers from the response
                    foreach (var header in optionsResponse.Headers)
                    {
                        results.AppendLine($"Header: {header.Key} = {string.Join(", ", header.Value)}");
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"OPTIONS error: {ex.Message}");
                }
            
                // Try a simple GET request to check if endpoint exists
                try
                {
                    var getResponse = await client.GetAsync(HttpClientHelper.LoginUrl);
                    results.AppendLine($"GET response: {(int)getResponse.StatusCode} {getResponse.StatusCode}");
                }
                catch (Exception ex)
                {
                    results.AppendLine($"GET error: {ex.Message}");
                }
            
                return results.ToString();
            }
            catch (Exception ex)
            {
                results.AppendLine($"Test error: {ex.Message}");
                if (ex.InnerException != null)
                    results.AppendLine($"Inner error: {ex.InnerException.Message}");
            
                return results.ToString();
            }
        }
    }
}