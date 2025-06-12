using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;
using MauiBlazorWeb.Web.Data;
using MauiBlazorWeb.Web.Data.Repositories;

namespace MauiBlazorWeb.Web.Services
{
    public class EntryService : IEntryService
    {
        private readonly IEntryRepository _entryRepository;

        public EntryService(IEntryRepository entryRepository)
        {
            _entryRepository = entryRepository;
        }

        public async Task<IEnumerable<EntryDto>> GetEntriesByShowClassIdAsync(string showClassId)
        {
            var entries = await _entryRepository.GetAllByShowClassIdAsync(showClassId);
            return entries.Select(MapToDto);
        }

        public async Task<IEnumerable<EntryDto>> GetEntriesByUserModelObjectIdAsync(string userModelObjectId)
        {
            var entries = await _entryRepository.GetAllByUserModelObjectIdAsync(userModelObjectId);
            return entries.Select(MapToDto);
        }

        public async Task<EntryDto?> GetEntryByIdAsync(string id)
        {
            var entry = await _entryRepository.GetWithResultAsync(id);
            return entry != null ? MapToDto(entry) : null;
        }

        public async Task<EntryDto> CreateEntryAsync(EntryDto entryDto)
        {
            if (!int.TryParse(entryDto.UserModelObjectId, out var userModelObjectId))
                throw new ArgumentException("Invalid user model object ID");
                
            if (!int.TryParse(entryDto.ShowClassId, out var showClassId))
                throw new ArgumentException("Invalid show class ID");

            var entry = new Entry
            {
                EntryNumber = entryDto.EntryNumber,
                SubmissionDate = entryDto.SubmissionDate,
                Status = Enum.Parse<EntryStatus>(entryDto.Status),
                UserModelObjectId = userModelObjectId,
                ShowClassId = showClassId
            };

            var result = await _entryRepository.CreateAsync(entry);
            return MapToDto(result);
        }

        public async Task<EntryDto> UpdateEntryAsync(EntryDto entryDto)
        {
            if (!int.TryParse(entryDto.Id, out var entryId))
                throw new ArgumentException("Invalid entry ID");
                
            if (!int.TryParse(entryDto.UserModelObjectId, out var userModelObjectId))
                throw new ArgumentException("Invalid user model object ID");
                
            if (!int.TryParse(entryDto.ShowClassId, out var showClassId))
                throw new ArgumentException("Invalid show class ID");

            var entry = await _entryRepository.GetByIdAsync(entryDto.Id);
            if (entry == null)
                throw new KeyNotFoundException($"Entry with ID {entryDto.Id} not found");

            entry.EntryNumber = entryDto.EntryNumber;
            entry.SubmissionDate = entryDto.SubmissionDate;
            entry.Status = Enum.Parse<EntryStatus>(entryDto.Status);
            entry.UserModelObjectId = userModelObjectId;
            entry.ShowClassId = showClassId;

            var result = await _entryRepository.UpdateAsync(entry);
            return MapToDto(result);
        }

        public async Task<bool> DeleteEntryAsync(string id)
        {
            return await _entryRepository.DeleteAsync(id);
        }

        private static EntryDto MapToDto(Entry entry)
        {
            return new EntryDto
            {
                Id = entry.Id.ToString(),
                EntryNumber = entry.EntryNumber,
                SubmissionDate = entry.SubmissionDate,
                Status = entry.Status.ToString(),
                UserModelObjectId = entry.UserModelObjectId.ToString(),
                HorseName = entry.UserModelObject?.Name ?? string.Empty,
                ShowClassId = entry.ShowClassId.ToString(),
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt
            };
        }
    }
}