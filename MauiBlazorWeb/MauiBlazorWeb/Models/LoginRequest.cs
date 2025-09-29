using System;
using System.ComponentModel.DataAnnotations;
using MauiBlazorWeb.Shared.Contracts.Auth.V1;

namespace MauiBlazorWeb.Models
{
    [Obsolete("Use MauiBlazorWeb.Shared.Contracts.Auth.V1.LoginRequest instead.")]
    public class LoginRequest
    {
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        public MauiBlazorWeb.Shared.Contracts.Auth.V1.LoginRequest ToContract() => new(Email, Password);

        public static LoginRequest FromContract(MauiBlazorWeb.Shared.Contracts.Auth.V1.LoginRequest c) => new()
        {
            Email = c.Email,
            Password = c.Password
        };
    }
}
