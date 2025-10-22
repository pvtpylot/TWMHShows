using MauiBlazorWeb.Models;

namespace MauiBlazorWeb.Services;

/// <summary>
///     Result of an authentication operation.
/// </summary>
public class AuthenticationResult
{
    /// <summary>
    ///     Indicates if the authentication was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    ///     Status message related to the authentication result.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    ///     JSON representation of the token data if authentication was successful.
    /// </summary>
    public string? TokenData { get; set; }
}

/// <summary>
///     Provides functionality for authenticating users and managing authentication tokens.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    ///     Authenticates a user with the provided credentials.
    /// </summary>
    /// <param name="loginRequest">The login credentials.</param>
    /// <returns>The result of the authentication attempt.</returns>
    Task<AuthenticationResult> AuthenticateAsync(LoginRequest loginRequest);

    /// <summary>
    ///     Attempts to refresh an access token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to use.</param>
    /// <returns>The result of the token refresh attempt.</returns>
    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken);

    /// <summary>
    ///     Revokes or invalidates authentication tokens for the specified user.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <returns>True if the logout was successful.</returns>
    Task<bool> LogoutAsync(string email);
}