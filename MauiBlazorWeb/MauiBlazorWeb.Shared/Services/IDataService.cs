using MauiBlazorWeb.Shared.Models.DTOs;

namespace MauiBlazorWeb.Shared.Services;

public interface IDataService
{
    Task<IEnumerable<UserModelObjectDto>> GetAllUserModelObjectsAsync(string? applicationUserId);
    Task<UserModelObjectDto> GetUserModelObjectByIdAsync(string id);
    Task<UserModelObjectDto> CreateUserModelObjectAsync(UserModelObjectDto userModelObject);
    Task<UserModelObjectDto> UpdateUserModelObjectAsync(string id, UserModelObjectDto userModelObject);
    Task<bool> DeleteUserModelObjectAsync(string id);
}