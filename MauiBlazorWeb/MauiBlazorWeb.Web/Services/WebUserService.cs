using MauiBlazorWeb.Shared.Models;
using MauiBlazorWeb.Shared.Services;
using MauiBlazorWeb.Web.Data;
using Microsoft.AspNetCore.Identity;

namespace MauiBlazorWeb.Web.Services;

public class WebUserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public WebUserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IUserInfo?> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
            return null;

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return null;

        return new UserInfo
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.First,
            LastName = user.Last
        };
    }

    public async Task<string> GetUserFirstNameAsync(string email)
    {
        var user = await GetUserByEmailAsync(email);
        return !string.IsNullOrEmpty(user?.FirstName) ? user.FirstName : "First_Name";
    }
}

// Simple implementation of IUserInfo
public class UserInfo : IUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}