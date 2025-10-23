using System.Collections.Generic;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;

namespace MauiBlazorWeb.Shared.Services
{
    public interface IDivisionService
    {
        Task<IEnumerable<DivisionDto>> GetAllByShowIdAsync(string showId);
        Task<DivisionDto?> GetByIdAsync(string id);
        Task<DivisionDto> CreateDivisionAsync(DivisionDto divisionDto);
        Task<DivisionDto> UpdateDivisionAsync(DivisionDto divisionDto);
        Task<bool> DeleteDivisionAsync(string id);
    }
}