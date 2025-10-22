using System.Diagnostics;
using System.Text.Json;
using MauiBlazorWeb.Models;

namespace MauiBlazorWeb.Services;

// Segregated interfaces following ISP
public interface ITokenReader
{
    Task<AccessTokenInfo?> GetTokenAsync();
    Task<bool> HasValidTokenAsync();
}

public interface ITokenWriter
{
    Task<AccessTokenInfo> SaveTokenAsync(string tokenJson, string email);
    Task RemoveTokenAsync();
}

public interface IUserPreferenceStorage
{
    Task<string?> GetSavedEmailAsync();
    Task SaveEmailPreferenceAsync(string email);
}

// Abstract storage provider following DIP
public interface ISecureStorageProvider
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
    void Remove(string key);
}

// Default implementation using MAUI SecureStorage
public class MauiSecureStorageProvider : ISecureStorageProvider
{
    public Task<string?> GetAsync(string key)
    {
        return SecureStorage.GetAsync(key);
    }

    public Task SetAsync(string key, string value)
    {
        return SecureStorage.SetAsync(key, value);
    }

    public void Remove(string key)
    {
        SecureStorage.Remove(key);
    }
}

public class TokenStorage : ITokenStorage
{
    private const string TokenKey = "auth_token";
    private const string RememberMeKey = "remember_me_email";
    private readonly JsonSerializerOptions _jsonOptions;

    private readonly ISecureStorageProvider _storageProvider;

    public TokenStorage(ISecureStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<AccessTokenInfo?> GetTokenAsync()
    {
        try
        {
            var tokenJson = await _storageProvider.GetAsync(TokenKey);

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
            await _storageProvider.SetAsync(TokenKey, tokenInfoJson);

            // Save the email address for remember me functionality
            await SaveEmailPreferenceAsync(email);

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
        _storageProvider.Remove(TokenKey);
        await Task.CompletedTask;
    }

    public async Task<bool> HasValidTokenAsync()
    {
        try
        {
            var token = await GetTokenAsync();
            if (token == null) return false;

            // Check if token is still valid
            return DateTime.UtcNow < token.AccessTokenExpiration;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetSavedEmailAsync()
    {
        return await _storageProvider.GetAsync(RememberMeKey);
    }

    public async Task SaveEmailPreferenceAsync(string email)
    {
        await _storageProvider.SetAsync(RememberMeKey, email);
    }
}

// Legacy facade for backward compatibility - should be marked as obsolete
public static class TokenStorageLegacy
{
    private static readonly Lazy<ITokenStorage> _tokenStorage = new(() =>
        new TokenStorage(new MauiSecureStorageProvider()));

    [Obsolete("Use dependency injection with ITokenStorage instead")]
    public static Task<AccessTokenInfo?> GetTokenFromSecureStorageAsync()
    {
        return _tokenStorage.Value.GetTokenAsync();
    }

    [Obsolete("Use dependency injection with ITokenStorage instead")]
    public static Task<AccessTokenInfo> SaveTokenToSecureStorageAsync(string tokenJson, string email)
    {
        return _tokenStorage.Value.SaveTokenAsync(tokenJson, email);
    }

    [Obsolete("Use dependency injection with ITokenStorage instead")]
    public static void RemoveToken()
    {
        _tokenStorage.Value.RemoveTokenAsync().Wait();
    }
}