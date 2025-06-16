using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using MauiBlazorWeb.Models;

namespace MauiBlazorWeb.Services
{
    public interface ITokenStorage
    {
        Task<AccessTokenInfo?> GetTokenAsync();
        Task<AccessTokenInfo> SaveTokenAsync(string tokenJson, string email);
        Task RemoveTokenAsync();
    }

    public class TokenStorage : ITokenStorage
    {
        private const string TokenKey = "auth_token";
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<AccessTokenInfo?> GetTokenAsync()
        {
            try
            {
                var tokenJson = await SecureStorage.GetAsync(TokenKey);
                
                if (string.IsNullOrEmpty(tokenJson))
                {
                    Debug.WriteLine("No token found in secure storage");
                    return null;
                }
                
                Debug.WriteLine("Token found in secure storage, deserializing");
                return JsonSerializer.Deserialize<AccessTokenInfo>(tokenJson, _jsonOptions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving token: {ex.Message}");
                await RemoveTokenAsync();
                return null;
            }
        }

        public async Task<AccessTokenInfo> SaveTokenAsync(string tokenJson, string email)
        {
            try
            {
                Debug.WriteLine("Saving token to secure storage");
                
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(tokenJson, _jsonOptions) 
                    ?? throw new InvalidOperationException("Failed to deserialize login response");

                // Calculate expiration (typically 1 hour from now)
                var expiresIn = loginResponse.ExpiresIn > 0 ? loginResponse.ExpiresIn : 3600;
                var expirationTime = DateTime.UtcNow.AddSeconds(expiresIn);
                
                var tokenInfo = new AccessTokenInfo
                {
                    LoginResponse = loginResponse,
                    AccessTokenExpiration = expirationTime,
                    Email = email
                };

                // Save the full token info
                var tokenInfoJson = JsonSerializer.Serialize(tokenInfo, _jsonOptions);
                await SecureStorage.SetAsync(TokenKey, tokenInfoJson);
                
                Debug.WriteLine($"Token saved successfully. Expires at: {expirationTime}");
                return tokenInfo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving token: {ex.Message}");
                throw;
            }
        }

        public async Task RemoveTokenAsync()
        {
            Debug.WriteLine("Removing token from secure storage");
            SecureStorage.Remove(TokenKey);
            await Task.CompletedTask;
        }
        
        // Static methods for backward compatibility
        public static Task<AccessTokenInfo?> GetTokenFromSecureStorageAsync()
        {
            var storage = new TokenStorage();
            return storage.GetTokenAsync();
        }
        
        public static Task<AccessTokenInfo> SaveTokenToSecureStorageAsync(string tokenJson, string email)
        {
            var storage = new TokenStorage();
            return storage.SaveTokenAsync(tokenJson, email);
        }
        
        public static void RemoveToken()
        {
            var storage = new TokenStorage();
            storage.RemoveTokenAsync().Wait();
        }
    }
}
