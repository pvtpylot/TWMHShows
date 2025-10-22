using MauiBlazorWeb.Shared.Models;

namespace MauiBlazorWeb.Shared.Services;

public interface IUserService
{
    Task<IUserInfo?> GetUserByEmailAsync(string email);
    Task<string> GetUserFirstNameAsync(string email);
}