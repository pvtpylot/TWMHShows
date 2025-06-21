using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MauiBlazorWeb.Models;

namespace MauiBlazorWeb.Services
{
    /// <summary>
    /// Default implementation of IAuthenticationService that uses HTTP requests to authenticate against the backend.
    /// </summary>
    public class DefaultAuthenticationService : IAuthenticationService
    {
        public async Task<AuthenticationResult> AuthenticateAsync(LoginRequest loginRequest)
        {
            try
            {
                var httpClient = HttpClientHelper.GetHttpClient();
                
                // Create form content
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", loginRequest.Email),
                    new KeyValuePair<string, string>("password", loginRequest.Password)
                });

                // Ensure Content-Type is set
                formContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                
                // Clear and set headers
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                
                // Make the request with a timeout
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                using var response = await httpClient.PostAsync(HttpClientHelper.LoginUrl, formContent, cts.Token);
                
                // Get response content regardless of status code
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return new AuthenticationResult
                {
                    Success = response.IsSuccessStatusCode,
                    Message = response.IsSuccessStatusCode ? "Authentication successful" : $"Authentication failed: {response.StatusCode}",
                    TokenData = response.IsSuccessStatusCode ? responseContent : null
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Authentication error: {ex.Message}");
                return new AuthenticationResult
                {
                    Success = false,
                    Message = $"Authentication error: {ex.Message}"
                };
            }
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var httpClient = HttpClientHelper.GetHttpClient();
                var refreshData = new { refreshToken };
                using var response = await httpClient.PostAsJsonAsync(HttpClientHelper.RefreshUrl, refreshData);
                
                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    return new AuthenticationResult
                    {
                        Success = true,
                        TokenData = token,
                        Message = "Token refreshed successfully"
                    };
                }
                else
                {
                    return new AuthenticationResult
                    {
                        Success = false,
                        Message = $"Failed to refresh token: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error refreshing token: {ex.Message}");
                return new AuthenticationResult
                {
                    Success = false,
                    Message = $"Error refreshing token: {ex.Message}"
                };
            }
        }

        public Task<bool> LogoutAsync(string email)
        {
            // Currently handled locally by removing the token from storage
            // Could be extended to call a server-side logout endpoint
            return Task.FromResult(true);
        }
    }
}