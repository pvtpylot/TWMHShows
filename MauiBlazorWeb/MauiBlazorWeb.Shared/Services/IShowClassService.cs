using MauiBlazorWeb.Shared.Models.DTOs;

namespace MauiBlazorWeb.Shared.Services;

public interface IShowClassService
{
    Task<IEnumerable<ShowClassDto>> GetClassesByShowIdAsync(string showId);
    Task<ShowClassDto?> GetShowClassByIdAsync(string id);
    Task<ShowClassDto> CreateShowClassAsync(ShowClassDto showClassDto);
    Task<ShowClassDto> UpdateShowClassAsync(ShowClassDto showClassDto);
    Task<bool> DeleteShowClassAsync(string id);
}