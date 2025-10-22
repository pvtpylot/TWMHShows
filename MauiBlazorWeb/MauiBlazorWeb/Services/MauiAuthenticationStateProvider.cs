using System.Diagnostics;
using System.Security.Claims;
using MauiBlazorWeb.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace MauiBlazorWeb.Services;

/// <summary>
///     This class manages the authentication state of the user.
///     The class handles user login, logout, and token validation, including refreshing tokens when they are close to
///     expiration.
///     It uses secure storage to save and retrieve tokens, ensuring that users do not need to log in every time.
/// </summary>
public class MauiAuthenticationStateProvider : AuthenticationStateProvider
{
    //TODO: Place this in AppSettings or Client config file
    private const string AuthenticationType = "Custom authentication";
    private const int TokenExpirationBuffer = 30; //minutes

    private static readonly ClaimsPrincipal _defaultUser = new(new ClaimsIdentity());

    private static readonly Task<AuthenticationState> _defaultAuthState
        = Task.FromResult(new AuthenticationState(_defaultUser));

    private readonly ITokenStorage _tokenStorage;
    private readonly INetworkDiagnostics _networkDiagnostics;
    private readonly IAuthenticationService _authService;

    public LoginStatus LoginStatus { get; private set; } = LoginStatus.None;
    public string LoginFailureMessage { get; private set; } = "";

    private Task<AuthenticationState> _currentAuthState = _defaultAuthState;
    private AccessTokenInfo? _accessToken;

    public MauiAuthenticationStateProvider(
        ITokenStorage tokenStorage,
        INetworkDiagnostics networkDiagnostics,
        IAuthenticationService authService)
    {
        _tokenStorage = tokenStorage;
        _networkDiagnostics = networkDiagnostics;
        _authService = authService;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_currentAuthState != _defaultAuthState) return _currentAuthState;

        _currentAuthState = CreateAuthenticationStateFromSecureStorageAsync();
        NotifyAuthenticationStateChanged(_currentAuthState);

        return _currentAuthState;
    }

    public async Task<AccessTokenInfo?> GetAccessTokenInfoAsync()
    {
        if (await UpdateAndValidateAccessTokenAsync()) return _accessToken;

        await Logout();
        return null;
    }

    public async Task Logout()
    {
        LoginStatus = LoginStatus.None;
        _currentAuthState = _defaultAuthState;
        _accessToken = null;
        await _tokenStorage.RemoveTokenAsync();
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
                NotifyAuthenticationStateChanged(_currentAuthState);
            else
                // If login failed but didn't throw an exception, notify with default auth state
                NotifyAuthenticationStateChanged(_defaultAuthState);
        }
        catch (HttpRequestException ex)
        {
            // This will catch network-related issues including SSL/certificate problems
            HandleNetworkException(ex);
        }
        catch (Exception ex)
        {
            // Catch any other unexpected errors
            HandleGeneralException(ex);
        }

        async Task<AuthenticationState> LogInAsyncCore(LoginRequest loginModel)
        {
            var user = await LoginWithProviderAsync(loginModel);
            return new AuthenticationState(user);
        }
    }

    private void HandleNetworkException(HttpRequestException ex)
    {
        LoginStatus = LoginStatus.Failed;
        LoginFailureMessage = $"Network error: {ex.Message}";
        Debug.WriteLine($"Login network error: {ex.Message}");

        if (ex.InnerException != null)
        {
            Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
            // Add more detailed error information for SSL/certificate issues
            if (ex.InnerException.Message.Contains("certificate") ||
                ex.InnerException.Message.Contains("SSL"))
                LoginFailureMessage = "SSL/Certificate error. Check your development certificate.";
        }

        NotifyAuthenticationStateChanged(_defaultAuthState);
    }

    private void HandleGeneralException(Exception ex)
    {
        LoginStatus = LoginStatus.Failed;
        LoginFailureMessage = "An unexpected error occurred.";
        Debug.WriteLine($"Login general error: {ex}");
        NotifyAuthenticationStateChanged(_defaultAuthState);
    }

    // Method to check if user has a valid session without doing a full login
    public async Task<bool> HasValidSessionAsync()
    {
        return await _tokenStorage.HasValidTokenAsync();
    }

    // Get previously used email from secure storage
    public async Task<string?> GetSavedEmailAsync()
    {
        return await _tokenStorage.GetSavedEmailAsync();
    }

    // Perform a diagnostic test on network connectivity
    public async Task<bool> PerformDiagnosticTest()
    {
        return await _networkDiagnostics.PerformConnectivityTestAsync();
    }

    private async Task<ClaimsPrincipal> LoginWithProviderAsync(LoginRequest loginModel)
    {
        var authenticatedUser = _defaultUser;
        LoginStatus = LoginStatus.None;

        try
        {
            // Run a diagnostic test first
            var diagnosticResult = await _networkDiagnostics.PerformConnectivityTestAsync();
            Debug.WriteLine($"Diagnostic test result: {(diagnosticResult ? "Success" : "Failed")}");

            // Perform authentication via service
            var loginResult = await _authService.AuthenticateAsync(loginModel);

            LoginStatus = loginResult.Success ? LoginStatus.Success : LoginStatus.Failed;
            LoginFailureMessage = loginResult.Message ?? string.Empty;

            if (LoginStatus == LoginStatus.Success)
            {
                Debug.WriteLine("Login successful, saving token");
                _accessToken = await _tokenStorage.SaveTokenAsync(loginResult.TokenData!, loginModel.Email);
                authenticatedUser = CreateAuthenticatedUser(loginModel.Email);
                Debug.WriteLine("Authentication state created successfully");
            }
            else
            {
                Debug.WriteLine($"Authentication failed: {LoginFailureMessage}");
            }
        }
        catch (TaskCanceledException ex)
        {
            HandleTimeoutException(ex);
        }
        catch (HttpRequestException ex)
        {
            HandleHttpRequestException(ex);
        }
        catch (Exception ex)
        {
            HandleGeneralLoginException(ex);
        }

        return authenticatedUser;
    }

    private void HandleTimeoutException(TaskCanceledException ex)
    {
        Debug.WriteLine($"Request timed out: {ex.Message}");
        LoginFailureMessage = "Login request timed out. Please check your network connection and server availability.";
        LoginStatus = LoginStatus.Failed;
    }

    private void HandleHttpRequestException(HttpRequestException ex)
    {
        Debug.WriteLine($"HTTP request error: {ex.Message}");
        LoginFailureMessage = $"Network error: {ex.Message}";

        if (ex.InnerException != null)
        {
            Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
            Debug.WriteLine($"Inner exception type: {ex.InnerException.GetType().Name}");

            // Add more detailed error information for SSL/certificate issues
            if (ex.InnerException.Message.Contains("certificate") ||
                ex.InnerException.Message.Contains("SSL") ||
                ex.InnerException.Message.Contains("trust"))
                LoginFailureMessage
                    = "SSL/Certificate error. This is common in development environments. Please check your development certificate setup.";
        }

        LoginStatus = LoginStatus.Failed;
    }

    private void HandleGeneralLoginException(Exception ex)
    {
        Debug.WriteLine($"Login general error: {ex.GetType().Name}: {ex.Message}");
        LoginFailureMessage = $"Error: {ex.Message}";
        LoginStatus = LoginStatus.Failed;
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
                _accessToken = await _tokenStorage.GetTokenAsync();

            if (_accessToken is null) return false;

            // The refresh token expiration is unknown, so we always try to refresh even if the access token expires. It defaults to 14 days.
            // However, we start trying to refresh the access token 30 minutes before it expires to avoid race conditions.
            if (thirtyMinutesFromNow >= _accessToken.AccessTokenExpiration)
                return await RefreshAccessTokenAsync(_accessToken.LoginResponse.RefreshToken, _accessToken.Email);

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
                var refreshResult = await _authService.RefreshTokenAsync(refreshToken);

                if (refreshResult.Success && refreshResult.TokenData != null)
                {
                    _accessToken = await _tokenStorage.SaveTokenAsync(refreshResult.TokenData, email);
                    return true;
                }
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
        // Build claims from the persisted access token so pages can find NameIdentifier and Roles
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, email),
            new(ClaimTypes.Email, email)
        };

        var userId = _accessToken?.LoginResponse?.UserId;
        if (!string.IsNullOrWhiteSpace(userId)) claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));

        var roles = _accessToken?.LoginResponse?.Roles;
        if (roles is not null && roles.Length > 0)
            foreach (var role in roles)
                if (!string.IsNullOrWhiteSpace(role))
                    claims.Add(new Claim(ClaimTypes.Role, role));

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