namespace MauiBlazorWeb.Shared.Contracts.Auth.V1;

/// <summary>
/// Contains token related metadata for a signed-in user.
/// </summary>
public sealed record AccessTokenInfo(
    string Email,
    LoginResponse LoginResponse,
    DateTime AccessTokenExpiration);
