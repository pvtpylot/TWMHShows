using System;
using MauiBlazorWeb.Shared.Contracts.Auth.V1;

namespace MauiBlazorWeb.Models
{
    /// <summary>
    /// (Deprecated) Use Shared.Contracts.Auth.V1.AccessTokenInfo instead.
    /// </summary>
    [Obsolete("Use MauiBlazorWeb.Shared.Contracts.Auth.V1.AccessTokenInfo instead.")]
    public class AccessTokenInfo
    {
        public required string Email { get; set; }
        public required LoginResponse LoginResponse { get; set; }
        public required DateTime AccessTokenExpiration { get; set; }

        public MauiBlazorWeb.Shared.Contracts.Auth.V1.AccessTokenInfo ToContract() =>
            new(Email, LoginResponse.ToContract(), AccessTokenExpiration);

        public static AccessTokenInfo FromContract(MauiBlazorWeb.Shared.Contracts.Auth.V1.AccessTokenInfo c) => new()
        {
            Email = c.Email,
            LoginResponse = LoginResponse.FromContract(c.LoginResponse),
            AccessTokenExpiration = c.AccessTokenExpiration
        };
    }
}
