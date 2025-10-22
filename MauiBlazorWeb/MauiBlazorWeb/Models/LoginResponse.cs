using System.Text.Json.Serialization;

namespace MauiBlazorWeb.Models;

/// <summary>
///     This class is used to store the token information received from the server.
/// </summary>
public class LoginResponse
{
    [JsonPropertyName("tokenType")] public required string TokenType { get; set; }

    [JsonPropertyName("accessToken")] public required string AccessToken { get; set; }

    [JsonPropertyName("expiresIn")] public required int ExpiresIn { get; set; }

    [JsonPropertyName("refreshToken")] public required string RefreshToken { get; set; }

    [JsonPropertyName("userId")] public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("roles")] public string[]? Roles { get; set; }
}