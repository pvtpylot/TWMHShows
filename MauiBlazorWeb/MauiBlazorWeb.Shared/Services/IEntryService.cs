using System.Collections.Generic;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;

namespace MauiBlazorWeb.Shared.Services
{
    public interface IEntryService
    {
        Task<IEnumerable<EntryDto>> GetEntriesByShowClassIdAsync(string showClassId);
        Task<IEnumerable<EntryDto>> GetEntriesByUserModelObjectIdAsync(string userModelObjectId);
        Task<EntryDto?> GetEntryByIdAsync(string id);
        Task<EntryDto> CreateEntryAsync(EntryDto entryDto);
        Task<EntryDto> UpdateEntryAsync(EntryDto entryDto);
        Task<bool> DeleteEntryAsync(string id);
    }
}