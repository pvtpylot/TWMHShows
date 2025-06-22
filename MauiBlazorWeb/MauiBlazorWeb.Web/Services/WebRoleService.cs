using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;
using MauiBlazorWeb.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MauiBlazorWeb.Web.Services
{
    public class WebRoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public WebRoleService(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Array.Empty<string>();
                
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> AddUserToRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;
                
            // Check if role exists
            if (!await _roleManager.RoleExistsAsync(role))
                return false;
                
            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;
        }

        public async Task<bool> RemoveUserFromRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;
                
            var result = await _userManager.RemoveFromRoleAsync(user, role);
            return result.Succeeded;
        }
        
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _userManager.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    FirstName = u.First,
                    LastName = u.Last,
                    IsLockedOut = u.LockoutEnabled && u.LockoutEnd > DateTimeOffset.Now,
                    EmailConfirmed = u.EmailConfirmed
                })
                .ToListAsync();
        }
        
        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;
                
            var roles = await _userManager.GetRolesAsync(user);
            
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.First,
                LastName = user.Last,
                IsLockedOut = user.LockoutEnabled && user.LockoutEnd > DateTimeOffset.Now,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToList()
            };
        }
    }
}