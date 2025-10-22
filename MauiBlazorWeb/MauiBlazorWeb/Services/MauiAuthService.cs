using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;
using MauiBlazorWeb.Models;
using MauiBlazorWeb.Shared.Services;

namespace MauiBlazorWeb.Services;

public class MauiAuthService : IAuthService
{
    private readonly MauiAuthenticationStateProvider _authStateProvider;
    private readonly HttpClient _httpClient;
    private readonly ITokenStorage _tokenStorage;
    private string? _currentUserId;
    private string[]? _currentUserRoles;

    public MauiAuthService(
        HttpClient httpClient,
        ITokenStorage tokenStorage,
        MauiAuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _tokenStorage = tokenStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var loginRequest = new LoginRequest { Email = email, Password = password };
            await _authStateProvider.LogInAsync(loginRequest);

            if (_authStateProvider.LoginStatus == LoginStatus.Success)
            {
                var tokenInfo = await _authStateProvider.GetAccessTokenInfoAsync();
                if (tokenInfo != null)
                {
                    _currentUserId = tokenInfo.LoginResponse.UserId;

                    var accessToken = tokenInfo.LoginResponse.AccessToken;
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        if (IsJwtExpired(accessToken))
                        {
                            await ClearTokenAsync();
                            return false;
                        }

                        _httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", accessToken);
                        await StoreValueAsync("access_token", accessToken);
                    }

                    if (tokenInfo.LoginResponse.Roles?.Any() == true)
                    {
                        _currentUserRoles = tokenInfo.LoginResponse.Roles;
                        await StoreValueAsync("user_roles", JsonSerializer.Serialize(_currentUserRoles));
                    }

                    await StoreValueAsync("user_id", _currentUserId);
                    return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> LogoutAsync()
    {
        try
        {
            await _authStateProvider.Logout();

            // Clear headers and cached values
            _httpClient.DefaultRequestHeaders.Authorization = null;
            await RemoveValueAsync("access_token");
            await RemoveValueAsync("user_id");
            await RemoveValueAsync("user_roles");

            _currentUserRoles = null;
            _currentUserId = null;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        await EnsureAuthHeaderAsync();
        return await _authStateProvider.HasValidSessionAsync();
    }

    public async Task<bool> EnsureAuthenticatedAsync()
    {
        await EnsureAuthHeaderAsync();
        return await IsAuthenticatedAsync();
    }

    public async Task<string?> GetCurrentUserIdAsync()
    {
        await EnsureAuthHeaderAsync();

        if (_currentUserId != null)
            return _currentUserId;

        _currentUserId = await RetrieveValueAsync("user_id");

        if (string.IsNullOrEmpty(_currentUserId))
        {
            var tokenInfo = await _authStateProvider.GetAccessTokenInfoAsync();
            if (tokenInfo != null)
            {
                _currentUserId = tokenInfo.LoginResponse.UserId;
                await StoreValueAsync("user_id", _currentUserId);
            }
        }

        return _currentUserId;
    }

    public async Task<string[]?> GetCurrentUserRolesAsync()
    {
        await EnsureAuthHeaderAsync();

        if (_currentUserRoles != null)
            return _currentUserRoles;

        var rolesJson = await RetrieveValueAsync("user_roles");
        if (!string.IsNullOrEmpty(rolesJson))
            try
            {
                _currentUserRoles = JsonSerializer.Deserialize<string[]>(rolesJson);
                return _currentUserRoles;
            }
            catch
            {
            }

        var tokenInfo = await _authStateProvider.GetAccessTokenInfoAsync();
        if (tokenInfo?.LoginResponse?.Roles?.Any() == true)
        {
            _currentUserRoles = tokenInfo.LoginResponse.Roles;
            await StoreValueAsync("user_roles", JsonSerializer.Serialize(_currentUserRoles));
        }

        return _currentUserRoles;
    }

    public async Task<bool> IsInRoleAsync(string role)
    {
        await EnsureAuthHeaderAsync();
        var roles = await GetCurrentUserRolesAsync();
        return roles?.Contains(role) == true;
    }

    public bool IsInRoleSync(string role)
    {
        return _currentUserRoles?.Contains(role) == true;
    }

    private async Task EnsureAuthHeaderAsync()
    {
        // If already set, verify token hasn't expired
        var existing = _httpClient.DefaultRequestHeaders.Authorization?.Parameter;
        if (!string.IsNullOrEmpty(existing) && IsJwtExpired(existing))
        {
            await ClearTokenAsync();
            return;
        }

        if (_httpClient.DefaultRequestHeaders.Authorization == null)
        {
            // Try from auth provider first, then from storage
            var tokenInfo = await _authStateProvider.GetAccessTokenInfoAsync();
            var token = tokenInfo?.LoginResponse?.AccessToken ?? await RetrieveValueAsync("access_token");
            if (!string.IsNullOrEmpty(token))
            {
                if (IsJwtExpired(token))
                {
                    await ClearTokenAsync();
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }

    // ADDED: checks JWT expiration with a small negative skew (treat near-expiry as expired)
    private static bool IsJwtExpired(string token, TimeSpan? clockSkew = null)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var skew = clockSkew ?? TimeSpan.FromSeconds(60);
            return jwt.ValidTo <= DateTime.UtcNow.Add(-skew);
        }
        catch
        {
            // Malformed/unreadable token -> treat as expired
            return true;
        }
    }

    // ADDED: centralized token/header cleanup
    private async Task ClearTokenAsync()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
        SecureStorage.Default.Remove("access_token");
        SecureStorage.Default.Remove("user_id");
        SecureStorage.Default.Remove("user_roles");
        await Task.CompletedTask;
    }

    private async Task StoreValueAsync(string key, string value)
    {
        await SecureStorage.Default.SetAsync(key, value);
    }

    private async Task<string> RetrieveValueAsync(string key)
    {
        return await SecureStorage.Default.GetAsync(key) ?? string.Empty;
    }

    private async Task RemoveValueAsync(string key)
    {
        SecureStorage.Default.Remove(key);
        await Task.CompletedTask;
    }
}