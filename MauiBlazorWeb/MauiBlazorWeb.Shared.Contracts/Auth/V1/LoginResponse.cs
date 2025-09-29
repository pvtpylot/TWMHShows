using System.Text.Json.Serialization;

namespace MauiBlazorWeb.Shared.Contracts.Auth.V1;

/// <summary>
/// Response returned after successful authentication.
/// </summary>
public sealed record LoginResponse(
    [property: JsonPropertyName("tokenType")] string TokenType,
    [property: JsonPropertyName("accessToken")] string AccessToken,
    [property: JsonPropertyName("expiresIn")] int ExpiresIn,
    [property: JsonPropertyName("refreshToken")] string RefreshToken,
    [property: JsonPropertyName("userId")] string UserId,
    [property: JsonPropertyName("roles")] string[]? Roles);
