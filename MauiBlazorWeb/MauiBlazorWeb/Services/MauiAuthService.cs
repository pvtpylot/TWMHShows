using MauiBlazorWeb.Shared.Models;
using MauiBlazorWeb.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Text.Json;
using MauiBlazorWeb.Models;

namespace MauiBlazorWeb.Services
{
    public class MauiAuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorage _tokenStorage;
        private readonly MauiAuthenticationStateProvider _authStateProvider;
        private string[]? _currentUserRoles;
        private string? _currentUserId;

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
                // Create the login request
                var loginRequest = new LoginRequest
                {
                    Email = email,
                    Password = password
                };

                // Use the authentication service indirectly through the state provider
                await _authStateProvider.LogInAsync(loginRequest);
                
                // Check if login was successful
                if (_authStateProvider.LoginStatus == LoginStatus.Success)
                {
                    // Get token info to retrieve roles and user ID
                    var tokenInfo = await _authStateProvider.GetAccessTokenInfoAsync();
                    if (tokenInfo != null)
                    {
                        // Extract and store user ID and roles from claims if available
                        _currentUserId = tokenInfo.LoginResponse.UserId;
                        
                        if (tokenInfo.LoginResponse.Roles?.Any() == true)
                        {
                            _currentUserRoles = tokenInfo.LoginResponse.Roles;
                            // Store roles as json string for persistence
                            await StoreValueAsync("user_roles", JsonSerializer.Serialize(_currentUserRoles));
                        }
                        
                        // Store user ID for persistence
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
                
                // Also clear our cached values
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
            return await _authStateProvider.HasValidSessionAsync();
        }

        public async Task<bool> EnsureAuthenticatedAsync()
        {
            return await IsAuthenticatedAsync();
        }

        public async Task<string?> GetCurrentUserIdAsync()
        {
            if (_currentUserId != null)
                return _currentUserId;
                
            // Try to get from storage
            _currentUserId = await RetrieveValueAsync("user_id");
            
            // If not in storage, try to get from the auth provider
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
            if (_currentUserRoles != null)
                return _currentUserRoles;

            // Try to get from storage
            var rolesJson = await RetrieveValueAsync("user_roles");
            if (!string.IsNullOrEmpty(rolesJson))
            {
                try
                {
                    _currentUserRoles = JsonSerializer.Deserialize<string[]>(rolesJson);
                    return _currentUserRoles;
                }
                catch
                {
                    // Failed to deserialize, continue to try other methods
                }
            }
            
            // If not in storage, try to get from the auth provider
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
            var roles = await GetCurrentUserRolesAsync();
            return roles?.Contains(role) == true;
        }

        public bool IsInRoleSync(string role)
        {
            // This is a synchronous version for UI bindings
            // It will use cached roles if available
            return _currentUserRoles?.Contains(role) == true;
        }
        
        // Helper methods to abstract the storage implementation
        private async Task StoreValueAsync(string key, string value)
        {
            // Use SecureStorage directly since ITokenStorage interface doesn't match
            await SecureStorage.Default.SetAsync(key, value);
        }
        
        private async Task<string> RetrieveValueAsync(string key)
        {
            return await SecureStorage.Default.GetAsync(key) ?? string.Empty;
        }
        
        private async Task RemoveValueAsync(string key)
        {
            SecureStorage.Default.Remove(key);
            await Task.CompletedTask; // To make it async consistent
        }
    }
}