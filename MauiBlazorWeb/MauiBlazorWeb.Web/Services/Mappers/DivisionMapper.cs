using System.Linq;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Web.Data;

namespace MauiBlazorWeb.Web.Services.Mappers
{
    public class DivisionMapper : IEntityMapper<Division, DivisionDto>
    {
        public DivisionDto MapToDto(Division entity)
        {
            return new DivisionDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                Description = entity.Description,
                DivisionType = entity.DivisionType.ToString(),
                SortOrder = entity.SortOrder,
                ShowId = entity.ShowId,
                ShowClasses = entity.ShowClasses?.OrderBy(sc => sc.SortOrder)
                    .ThenBy(sc => sc.ClassNumber)
                    .Select(MapClassToDto).ToList() ?? new(),
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public Division MapToEntity(DivisionDto dto)
        {
            return new Division
            {
                Id = long.TryParse(dto.Id, out var id) ? id : 0,
                Name = dto.Name,
                Description = dto.Description,
                DivisionType = Enum.TryParse<DivisionType>(dto.DivisionType, true, out var t) ? t : DivisionType.Halter,
                SortOrder = dto.SortOrder,
                ShowId = dto.ShowId,
            };
        }

        public void MapToExistingEntity(DivisionDto dto, Division existing)
        {
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.SortOrder = dto.SortOrder;
            existing.DivisionType = Enum.TryParse<DivisionType>(dto.DivisionType, true, out var t) ? t : existing.DivisionType;
            existing.ShowId = dto.ShowId;
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
    }
}