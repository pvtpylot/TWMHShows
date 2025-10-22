using System.Collections.Generic;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;

namespace MauiBlazorWeb.Shared.Services
{
    public interface IShowService
    {
        Task<IEnumerable<ShowDto>> GetAllShowsAsync();
        Task<IEnumerable<ShowDto>> GetShowsByJudgeIdAsync(string judgeId);
        Task<ShowDto?> GetShowByIdAsync(string id);
        Task<ShowDto> CreateShowAsync(ShowDto showDto);
        Task<ShowDto> UpdateShowAsync(ShowDto showDto);
        Task<bool> DeleteShowAsync(string id);
    }
}