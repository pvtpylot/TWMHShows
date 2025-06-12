using MauiBlazorWeb.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Security.Claims;

namespace MauiBlazorWeb.Services
{
    /// <summary>
    /// This class manages the authentication state of the user.
    /// The class handles user login, logout, and token validation, including refreshing tokens when they are close to expiration.
    /// It uses secure storage to save and retrieve tokens, ensuring that users do not need to log in every time.
    /// </summary>
    public class MauiAuthenticationStateProvider : AuthenticationStateProvider
    {
        //TODO: Place this in AppSettings or Client config file
        private const string AuthenticationType = "Custom authentication";
        private const int TokenExpirationBuffer = 30; //minutes

        private static ClaimsPrincipal _defaultUser = new ClaimsPrincipal(new ClaimsIdentity());
        private static Task<AuthenticationState> _defaultAuthState = Task.FromResult(new AuthenticationState(_defaultUser));

        public LoginStatus LoginStatus { get; set; } = LoginStatus.None;
        public string LoginFailureMessage { get; set; } = "";

        private Task<AuthenticationState> _currentAuthState = _defaultAuthState;
        private AccessTokenInfo? _accessToken;

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_currentAuthState != _defaultAuthState)
            {
                return _currentAuthState;
            }

            _currentAuthState = CreateAuthenticationStateFromSecureStorageAsync();
            NotifyAuthenticationStateChanged(_currentAuthState);

            return _currentAuthState;
        }

        public async Task<AccessTokenInfo?> GetAccessTokenInfoAsync()
        {
            if (await UpdateAndValidateAccessTokenAsync())
            {
                return _accessToken;
            }

            Logout();
            return null;
        }

        public void Logout()
        {
            LoginStatus = LoginStatus.None;
            _currentAuthState = _defaultAuthState;
            _accessToken = null;
            TokenStorage.RemoveToken();
            NotifyAuthenticationStateChanged(_defaultAuthState);
        }

        public async Task LogInAsync(LoginRequest loginModel)
        {
            try
            {
                var authState = await LogInAsyncCore(loginModel);
                _currentAuthState = Task.FromResult(authState);
        
                // Only notify if login was successful
                if (LoginStatus == LoginStatus.Success)
                {
                    NotifyAuthenticationStateChanged(_currentAuthState);
                }
                else
                {
                    // If login failed but didn't throw an exception, notify with default auth state
                    NotifyAuthenticationStateChanged(_defaultAuthState);
                }
            }
            catch (HttpRequestException ex)
            {
                // This will catch network-related issues including SSL/certificate problems
                LoginStatus = LoginStatus.Failed;
                LoginFailureMessage = $"Network error: {ex.Message}";
                Debug.WriteLine($"Login network error: {ex.Message}");
        
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    // Add more detailed error information for SSL/certificate issues
                    if (ex.InnerException.Message.Contains("certificate") || 
                        ex.InnerException.Message.Contains("SSL"))
                    {
                        LoginFailureMessage = "SSL/Certificate error. Check your development certificate.";
                    }
                }
        
                NotifyAuthenticationStateChanged(_defaultAuthState);
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors
                LoginStatus = LoginStatus.Failed;
                LoginFailureMessage = "An unexpected error occurred.";
                Debug.WriteLine($"Login general error: {ex}");
                NotifyAuthenticationStateChanged(_defaultAuthState);
            }

            async Task<AuthenticationState> LogInAsyncCore(LoginRequest loginModel)
            {
                var user = await LoginWithProviderAsync(loginModel);
                return new AuthenticationState(user);
            }
        }

        private async Task<ClaimsPrincipal> LoginWithProviderAsync(LoginRequest loginModel)
{
    var authenticatedUser = _defaultUser;
    LoginStatus = LoginStatus.None;

    try
    {
        // Create HttpClient with detailed debugging
        var httpClient = HttpClientHelper.GetHttpClient();
        Debug.WriteLine($"Attempting login to URL: {HttpClientHelper.LoginUrl}");
        
        var loginData = new { loginModel.Email, loginModel.Password };
        Debug.WriteLine($"Login attempt for: {loginModel.Email}");
        
        using var response = await httpClient.PostAsJsonAsync(HttpClientHelper.LoginUrl, loginData);
        
        Debug.WriteLine($"Login response status: {(int)response.StatusCode} {response.StatusCode}");
        
        // Get the full response content for debugging
        var responseContent = await response.Content.ReadAsStringAsync();
        Debug.WriteLine($"Response content: {responseContent}");

        LoginStatus = response.IsSuccessStatusCode ? LoginStatus.Success : LoginStatus.Failed;

        if (LoginStatus == LoginStatus.Success)
        {
            // Save token to secure storage so the user doesn't have to login every time
            _accessToken = await TokenStorage.SaveTokenToSecureStorageAsync(responseContent, loginModel.Email);
            authenticatedUser = CreateAuthenticatedUser(loginModel.Email);
        }
        else
        {
            LoginFailureMessage = $"Login failed: {(int)response.StatusCode} {response.StatusCode}";
            
            if (!string.IsNullOrEmpty(responseContent))
            {
                LoginFailureMessage += $" - {responseContent}";
            }
            
            Debug.WriteLine($"Login failure: {LoginFailureMessage}");
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Login exception: {ex.GetType().Name}: {ex.Message}");
        
        if (ex.InnerException != null)
        {
            Debug.WriteLine($"Inner exception: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
        }
        
        LoginFailureMessage = $"Error: {ex.Message}";
        LoginStatus = LoginStatus.Failed;
    }

    return authenticatedUser;
}

        private async Task<AuthenticationState> CreateAuthenticationStateFromSecureStorageAsync()
        {
            var authenticatedUser = _defaultUser;
            LoginStatus = LoginStatus.None;

            if (await UpdateAndValidateAccessTokenAsync())
            {
                authenticatedUser = CreateAuthenticatedUser(_accessToken!.Email);
                LoginStatus = LoginStatus.Success;
            }

            return new AuthenticationState(authenticatedUser);
        }

        private async Task<bool> UpdateAndValidateAccessTokenAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var thirtyMinutesFromNow = now.AddMinutes(TokenExpirationBuffer);

                if (_accessToken is null || thirtyMinutesFromNow > _accessToken.AccessTokenExpiration)
                {
                    _accessToken = await TokenStorage.GetTokenFromSecureStorageAsync();
                }

                if (_accessToken is null)
                {
                    return false;
                }

                // The refresh token expiration is unknown, so we always try to refresh even if the access token expires. It defaults to 14 days.
                // However, we start trying to refresh the access token 30 minutes before it expires to avoid race conditions.
                if (thirtyMinutesFromNow >= _accessToken.AccessTokenExpiration)
                {
                    return await RefreshAccessTokenAsync(_accessToken.LoginResponse.RefreshToken, _accessToken.Email);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking token for validity: {ex}");
                return false;
            }
        }

        private async Task<bool> RefreshAccessTokenAsync(string refreshToken, string email)
        {
            try
            {
                if (refreshToken != null)
                {
                    //Call the Refresh endpoint and pass the refresh token
                    var httpClient = HttpClientHelper.GetHttpClient();
                    var refreshData = new { refreshToken };
                    using var response = await httpClient.PostAsJsonAsync(HttpClientHelper.RefreshUrl, refreshData);
                    response.EnsureSuccessStatusCode();
                    var token = await response.Content.ReadAsStringAsync();
                    _accessToken = await TokenStorage.SaveTokenToSecureStorageAsync(token, email);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error refreshing access token: {ex}");
                throw;
            }
        }

        private ClaimsPrincipal CreateAuthenticatedUser(string email)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, email) };  //TODO: Add more claims as needed
            var identity = new ClaimsIdentity(claims, AuthenticationType);
            return new ClaimsPrincipal(identity);
        }
        
#if IOS
        // Add this method to MauiAuthenticationStateProvider
        private async Task<bool> TestDirectHttpRequest()
        {
            try
            {
                // Create a basic HttpClient request without using the helper
                using var httpClient = new HttpClient(new NSUrlSessionHandler
                {
                    TrustOverrideForUrl = (_, url, _) => true
                });
                
                var response = await httpClient.GetAsync("https://localhost:7157/");
                Debug.WriteLine($"Test direct request result: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Test direct request failed: {ex.Message}");
                return false;
            }
        }
#endif
    }
}
