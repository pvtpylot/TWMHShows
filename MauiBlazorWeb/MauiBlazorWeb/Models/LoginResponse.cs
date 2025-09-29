using System.Text.Json.Serialization;
using System;
using MauiBlazorWeb.Shared.Contracts.Auth.V1;

namespace MauiBlazorWeb.Models
{
    /// <summary>
    /// (Deprecated) Use MauiBlazorWeb.Shared.Contracts.Auth.V1.LoginResponse instead.
    /// </summary>
    [Obsolete("Use MauiBlazorWeb.Shared.Contracts.Auth.V1.LoginResponse instead")]
    public class LoginResponse
    {
        [JsonPropertyName("tokenType")]
        public required string TokenType { get; set; }

        [JsonPropertyName("accessToken")]
        public required string AccessToken { get; set; }

        [JsonPropertyName("expiresIn")]
        public required int ExpiresIn { get; set; }

        [JsonPropertyName("refreshToken")]
        public required string RefreshToken { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("roles")]
        public string[]? Roles { get; set; }

        public MauiBlazorWeb.Shared.Contracts.Auth.V1.LoginResponse ToContract() => new(TokenType, AccessToken, ExpiresIn, RefreshToken, UserId, Roles);

        public static LoginResponse FromContract(MauiBlazorWeb.Shared.Contracts.Auth.V1.LoginResponse c) => new()
        {
            TokenType = c.TokenType,
            AccessToken = c.AccessToken,
            ExpiresIn = c.ExpiresIn,
            RefreshToken = c.RefreshToken,
            UserId = c.UserId,
            Roles = c.Roles
        };
    }
}
