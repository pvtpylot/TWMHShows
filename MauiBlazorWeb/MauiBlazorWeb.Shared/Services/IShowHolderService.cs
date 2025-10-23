using System.Collections.Generic;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;

namespace MauiBlazorWeb.Shared.Services
{
    public interface IShowHolderService
    {
        Task<IEnumerable<ShowDto>> GetMyShowsAsync(string userId, bool isAdmin = false);
        Task<ShowDto> CreateShowAsync(string userId, ShowDto showDto, bool isAdmin = false);
        Task<ShowDto> UpdateShowAsync(string userId, ShowDto showDto, bool isAdmin = false);
        Task<bool> DeleteShowAsync(string userId, string showId, bool isAdmin = false);
    }
}