using System.Collections.Generic;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;

namespace MauiBlazorWeb.Shared.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<string>> GetAllRolesAsync();
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task<bool> AddUserToRoleAsync(string userId, string role);
        Task<bool> RemoveUserFromRoleAsync(string userId, string role);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(string userId);
    }
}