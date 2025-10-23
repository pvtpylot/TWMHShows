using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Web.Data;
using MauiBlazorWeb.Web.Data.Repositories;

namespace MauiBlazorWeb.Web.Services
{
    public class DivisionService : MauiBlazorWeb.Shared.Services.IDivisionService
    {
        private readonly IDivisionRepository _divisionRepository;

        public DivisionService(IDivisionRepository divisionRepository)
        {
            _divisionRepository = divisionRepository;
        }

        public async Task<IEnumerable<DivisionDto>> GetAllByShowIdAsync(string showId)
        {
            if (!int.TryParse(showId, out var sid) || sid <= 0)
                return Enumerable.Empty<DivisionDto>();

            var divisions = await _divisionRepository.GetAllByShowIdAsync(sid);
            return (divisions ?? new List<Division>())
                .OrderBy(d => d.SortOrder)
                .ThenBy(d => d.Name)
                .Select(MapToDto);
        }

        public async Task<DivisionDto?> GetByIdAsync(string id)
        {
            if (!long.TryParse(id, out var did) || did <= 0 || did > int.MaxValue)
                return null;

            // Repository uses int in GetWithResultsAsync; cast safely while we standardize IDs.
            var division = await _divisionRepository.GetWithResultsAsync((int)did);
            return division is null ? null : MapToDto(division);
        }

        public async Task<DivisionDto> CreateDivisionAsync(DivisionDto divisionDto)
        {
            if (divisionDto == null)
                throw new ArgumentNullException(nameof(divisionDto));

            var entity = MapToEntity(divisionDto);
            entity.CreatedAt = DateTime.UtcNow;

            var created = await _divisionRepository.CreateAsync(entity);
            return MapToDto(created);
        }

        public async Task<DivisionDto> UpdateDivisionAsync(DivisionDto divisionDto)
        {
            if (divisionDto == null)
                throw new ArgumentNullException(nameof(divisionDto));

            if (!long.TryParse(divisionDto.Id, out var did) || did <= 0)
                throw new ArgumentException("Invalid division ID", nameof(divisionDto));

            var existing = await _divisionRepository.GetWithResultsAsync(unchecked((int)did));
            if (existing == null)
                throw new KeyNotFoundException($"Division with ID {divisionDto.Id} not found");

            // Update allowed fields
            existing.Name = divisionDto.Name;
            existing.Description = divisionDto.Description;
            existing.SortOrder = divisionDto.SortOrder;
            existing.DivisionType = ParseEnumOrDefault<DivisionType>(divisionDto.DivisionType, DivisionType.Halter);
            existing.ShowId = divisionDto.ShowId;

            var updated = await _divisionRepository.UpdateAsync(existing);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteDivisionAsync(string id)
        {
            if (!int.TryParse(id, out var did) || did <= 0)
                return false;

            // Soft delete per repository implementation
            return await _divisionRepository.DeleteAsync(did);
        }

        private static DivisionDto MapToDto(Division division)
        {
            return new DivisionDto
            {
                Id = division.Id.ToString(),
                Name = division.Name,
                Description = division.Description,
                DivisionType = division.DivisionType.ToString(),
                SortOrder = division.SortOrder,
                ShowId = division.ShowId,
                ShowClasses = division.ShowClasses?.OrderBy(sc => sc.SortOrder).ThenBy(sc => sc.ClassNumber).Select(MapClassToDto).ToList() ?? new(),
                CreatedAt = division.CreatedAt,
                UpdatedAt = division.UpdatedAt
            };
        }

        private static ShowClassDto MapClassToDto(ShowClass sc)
        {
            return new ShowClassDto
            {
                Id = sc.Id.ToString(),
                Name = sc.Name,
                Description = sc.Description,
                ClassNumber = sc.ClassNumber,
                MaxEntries = sc.MaxEntries,
                SortOrder = sc.SortOrder,
                BreedCategory = sc.BreedCategory?.ToString(),
                FinishType = sc.FinishType?.ToString(),
                PerformanceType = sc.PerformanceType?.ToString(),
                CollectibilityType = sc.CollectibilityType?.ToString(),
                GenderRestriction = sc.GenderRestriction?.ToString(),
                AgeRestriction = sc.AgeRestriction?.ToString(),
                ColorRestriction = sc.ColorRestriction?.ToString(),
                ScaleRestriction = sc.ScaleRestriction?.ToString(),
                DivisionId = sc.DivisionId,
                ShowId = sc.ShowId,
                CreatedAt = sc.CreatedAt,
                UpdatedAt = sc.UpdatedAt
            };
        }

        private static Division MapToEntity(DivisionDto dto)
        {
            return new Division
            {
                Id = long.TryParse(dto.Id, out var id) ? id : 0,
                Name = dto.Name,
                Description = dto.Description,
                DivisionType = ParseEnumOrDefault<DivisionType>(dto.DivisionType, DivisionType.Halter),
                SortOrder = dto.SortOrder,
                ShowId = dto.ShowId
            };
        }

        private static TEnum ParseEnumOrDefault<TEnum>(string? value, TEnum defaultValue) where TEnum : struct
        {
            return Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) ? parsed : defaultValue;
        }
    }
}