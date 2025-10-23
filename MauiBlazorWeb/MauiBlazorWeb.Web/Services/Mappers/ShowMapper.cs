using System.Linq;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Web.Data;

namespace MauiBlazorWeb.Web.Services.Mappers
{
    public class ShowMapper : IEntityMapper<Show, ShowDto>
    {
        private readonly IEntityMapper<Division, DivisionDto> _divisionMapper;

        public ShowMapper(IEntityMapper<Division, DivisionDto> divisionMapper)
        {
            _divisionMapper = divisionMapper;
        }

        public ShowDto MapToDto(Show entity)
        {
            return new ShowDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                Description = entity.Description,
                ShowDate = entity.ShowDate,
                EndDate = entity.EndDate,
                Status = entity.Status.ToString(),
                JudgeId = entity.JudgeId,
                JudgeName = entity.Judge?.UserName ?? string.Empty,
                ShowHolderId = entity.ShowHolderId,
                ShowHolderName = entity.ShowHolder?.Name ?? string.Empty,
                ShowType = entity.ShowType.ToString(),
                ShowFormat = entity.ShowFormat.ToString(),
                IsPrivate = entity.IsPrivate,
                MaxEntriesPerUser = entity.MaxEntriesPerUser,
                AllowMemberOnlyEntries = entity.AllowMemberOnlyEntries,
                EntryDeadline = entity.EntryDeadline,
                JudgingDeadline = entity.JudgingDeadline,
                ResultsPublishedAt = entity.ResultsPublishedAt,
                IsNanQualifying = entity.IsNanQualifying,
                NamhsaGuidelines = entity.NamhsaGuidelines,
                AdditionalMetadata = entity.AdditionalMetadata,
                Divisions = entity.Divisions?.Select(_divisionMapper.MapToDto).ToList() ?? new(),
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public Show MapToEntity(ShowDto dto)
        {
            return new Show
            {
                Id = int.TryParse(dto.Id, out var id) ? id : 0,
                Name = dto.Name,
                Description = dto.Description,
                ShowDate = dto.ShowDate,
                EndDate = dto.EndDate,
                Status = Enum.TryParse<ShowStatus>(dto.Status, true, out var s) ? s : ShowStatus.Upcoming,
                JudgeId = dto.JudgeId,
                ShowHolderId = dto.ShowHolderId,
                ShowType = Enum.TryParse<ShowType>(dto.ShowType, true, out var st) ? st : ShowType.LiveShow,
                ShowFormat = Enum.TryParse<ShowFormat>(dto.ShowFormat, true, out var sf) ? sf : ShowFormat.Regular,
                IsPrivate = dto.IsPrivate,
                MaxEntriesPerUser = dto.MaxEntriesPerUser,
                AllowMemberOnlyEntries = dto.AllowMemberOnlyEntries,
                EntryDeadline = dto.EntryDeadline,
                JudgingDeadline = dto.JudgingDeadline,
                ResultsPublishedAt = dto.ResultsPublishedAt,
                IsNanQualifying = dto.IsNanQualifying,
                NamhsaGuidelines = dto.NamhsaGuidelines,
                AdditionalMetadata = dto.AdditionalMetadata
            };
        }

        public void MapToExistingEntity(ShowDto dto, Show existing)
        {
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.ShowDate = dto.ShowDate;
            existing.EndDate = dto.EndDate;
            existing.Status = Enum.TryParse<ShowStatus>(dto.Status, true, out var s) ? s : existing.Status;
            existing.JudgeId = dto.JudgeId;
            existing.ShowHolderId = dto.ShowHolderId;
            existing.ShowType = Enum.TryParse<ShowType>(dto.ShowType, true, out var st) ? st : existing.ShowType;
            existing.ShowFormat = Enum.TryParse<ShowFormat>(dto.ShowFormat, true, out var sf) ? sf : existing.ShowFormat;
            existing.IsPrivate = dto.IsPrivate;
            existing.MaxEntriesPerUser = dto.MaxEntriesPerUser;
            existing.AllowMemberOnlyEntries = dto.AllowMemberOnlyEntries;
            existing.EntryDeadline = dto.EntryDeadline;
            existing.JudgingDeadline = dto.JudgingDeadline;
            existing.ResultsPublishedAt = dto.ResultsPublishedAt;
            existing.IsNanQualifying = dto.IsNanQualifying;
            existing.NamhsaGuidelines = dto.NamhsaGuidelines;
            existing.AdditionalMetadata = dto.AdditionalMetadata;
        }
    }
}