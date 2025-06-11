using MauiBlazorWeb.Shared.Models;
using MauiBlazorWeb.Shared.Services;

namespace MauiBlazorWeb.Services
{
    public class MauiUserService : IUserService
    {
        // In a real app, this would likely call an API endpoint
        public Task<IUserInfo?> GetUserByEmailAsync(string email)
        {
            // Simple implementation for now
            var userInfo = new MauiUserInfo
            {
                Id = "maui-user-id",
                Email = email,
                FirstName = "MAUI",
                LastName = "User"
            };
            
            return Task.FromResult<IUserInfo?>(userInfo);
        }

        public Task<string> GetUserFirstNameAsync(string email)
        {
            return Task.FromResult("MAUI User");
        }
    }

    // Simple implementation of IUserInfo
    public class MauiUserInfo : IUserInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}