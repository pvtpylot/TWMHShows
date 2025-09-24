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
    public class ShowService : IShowService
    {
        private readonly IShowRepository _showRepository;

        public ShowService(IShowRepository showRepository)
        {
            _showRepository = showRepository;
        }

        public async Task<IEnumerable<ShowDto>> GetAllShowsAsync()
        {
            var shows = await _showRepository.GetAllAsync();
            return shows.Select(MapToDto);
        }

        public async Task<IEnumerable<ShowDto>> GetShowsByJudgeIdAsync(string judgeId)
        {
            var shows = await _showRepository.GetAllByJudgeIdAsync(judgeId);
            return shows.Select(MapToDto);
        }

        public async Task<ShowDto?> GetShowByIdAsync(string id)
        {
            var show = await _showRepository.GetByIdAsync(id);
            return show != null ? MapToDto(show) : null;
        }

        public async Task<ShowDto> CreateShowAsync(ShowDto showDto)
        {
            var show = new Show
            {
                Name = showDto.Name,
                Description = showDto.Description,
                ShowDate = showDto.ShowDate,
                EndDate = showDto.EndDate,
                Status = Enum.Parse<ShowStatus>(showDto.Status, ignoreCase: true),
                JudgeId = showDto.JudgeId,
                ShowHolderId = showDto.ShowHolderId,
                ShowType = Enum.Parse<ShowType>(showDto.ShowType, ignoreCase: true),
                ShowFormat = Enum.Parse<ShowFormat>(showDto.ShowFormat, ignoreCase: true),
                IsPrivate = showDto.IsPrivate,
                MaxEntriesPerUser = showDto.MaxEntriesPerUser,
                AllowMemberOnlyEntries = showDto.AllowMemberOnlyEntries,
                EntryDeadline = showDto.EntryDeadline,
                JudgingDeadline = showDto.JudgingDeadline,
                ResultsPublishedAt = showDto.ResultsPublishedAt,
                IsNanQualifying = showDto.IsNanQualifying,
                NamhsaGuidelines = showDto.NamhsaGuidelines,
                AdditionalMetadata = showDto.AdditionalMetadata,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (showDto.Divisions?.Any() == true)
            {
                show.Divisions = showDto.Divisions.Select(d => new Division
                {
                    Name = d.Name,
                    Description = d.Description,
                    DivisionType = Enum.Parse<DivisionType>(d.DivisionType, ignoreCase: true),
                    SortOrder = d.SortOrder,
                    ShowId = show.Id,
                    ShowClasses = d.ShowClasses.Select(c => new ShowClass
                    {
                        Name = c.Name,
                        Description = c.Description,
                        ClassNumber = c.ClassNumber,
                        MaxEntries = c.MaxEntries,
                        SortOrder = c.SortOrder,
                        BreedCategory = ParseNullable<BreedCategory>(c.BreedCategory),
                        FinishType = ParseNullable<FinishType>(c.FinishType),
                        PerformanceType = ParseNullable<PerformanceType>(c.PerformanceType),
                        CollectibilityType = ParseNullable<CollectibilityType>(c.CollectibilityType),
                        GenderRestriction = ParseNullable<Gender>(c.GenderRestriction),
                        AgeRestriction = ParseNullable<AgeCategory>(c.AgeRestriction),
                        ColorRestriction = ParseNullable<ColorRestriction>(c.ColorRestriction),
                        ScaleRestriction = ParseNullable<ScaleCategory>(c.ScaleRestriction),
                        ShowId = show.Id
                    }).ToList()
                }).ToList();
            }

            var result = await _showRepository.CreateAsync(show);
            return MapToDto(result);
        }

        public async Task<ShowDto> UpdateShowAsync(ShowDto showDto)
        {
            if (!int.TryParse(showDto.Id, out var showId))
                throw new ArgumentException("Invalid show ID");

            var show = await _showRepository.GetByIdAsync(showDto.Id);
            if (show == null)
                throw new KeyNotFoundException($"Show with ID {showDto.Id} not found");

            show.Name = showDto.Name;
            show.Description = showDto.Description;
            show.ShowDate = showDto.ShowDate;
            show.EndDate = showDto.EndDate;
            show.Status = Enum.Parse<ShowStatus>(showDto.Status, ignoreCase: true);
            show.JudgeId = showDto.JudgeId;
            show.ShowHolderId = showDto.ShowHolderId;
            show.ShowType = Enum.Parse<ShowType>(showDto.ShowType, ignoreCase: true);
            show.ShowFormat = Enum.Parse<ShowFormat>(showDto.ShowFormat, ignoreCase: true);
            show.IsPrivate = showDto.IsPrivate;
            show.MaxEntriesPerUser = showDto.MaxEntriesPerUser;
            show.AllowMemberOnlyEntries = showDto.AllowMemberOnlyEntries;
            show.EntryDeadline = showDto.EntryDeadline;
            show.JudgingDeadline = showDto.JudgingDeadline;
            show.ResultsPublishedAt = showDto.ResultsPublishedAt;
            show.IsNanQualifying = showDto.IsNanQualifying;
            show.NamhsaGuidelines = showDto.NamhsaGuidelines;
            show.AdditionalMetadata = showDto.AdditionalMetadata;
            show.UpdatedAt = DateTime.UtcNow;

            // Replace divisions/classes for simplicity (ensure repository handles child graph updates)
            if (showDto.Divisions is not null)
            {
                show.Divisions = showDto.Divisions.Select(d => new Division
                {
                    Id = long.TryParse(d.Id, out var did) ? did : 0,
                    Name = d.Name,
                    Description = d.Description,
                    DivisionType = Enum.Parse<DivisionType>(d.DivisionType, ignoreCase: true),
                    SortOrder = d.SortOrder,
                    ShowId = show.Id,
                    ShowClasses = d.ShowClasses.Select(c => new ShowClass
                    {
                        Id = int.TryParse(c.Id, out var cid) ? cid : 0,
                        Name = c.Name,
                        Description = c.Description,
                        ClassNumber = c.ClassNumber,
                        MaxEntries = c.MaxEntries,
                        SortOrder = c.SortOrder,
                        BreedCategory = ParseNullable<BreedCategory>(c.BreedCategory),
                        FinishType = ParseNullable<FinishType>(c.FinishType),
                        PerformanceType = ParseNullable<PerformanceType>(c.PerformanceType),
                        CollectibilityType = ParseNullable<CollectibilityType>(c.CollectibilityType),
                        GenderRestriction = ParseNullable<Gender>(c.GenderRestriction),
                        AgeRestriction = ParseNullable<AgeCategory>(c.AgeRestriction),
                        ColorRestriction = ParseNullable<ColorRestriction>(c.ColorRestriction),
                        ScaleRestriction = ParseNullable<ScaleCategory>(c.ScaleRestriction),
                        DivisionId = long.TryParse(d.Id, out var divId) ? divId : 0,
                        ShowId = show.Id
                    }).ToList()
                }).ToList();
            }

            var result = await _showRepository.UpdateAsync(show);
            return MapToDto(result);
        }

        public async Task<bool> DeleteShowAsync(string id)
        {
            return await _showRepository.DeleteAsync(id);
        }

        private static ShowDto MapToDto(Show show)
        {
            return new ShowDto
            {
                Id = show.Id.ToString(),
                Name = show.Name,
                Description = show.Description,
                ShowDate = show.ShowDate,
                EndDate = show.EndDate,
                Status = show.Status.ToString(),
                JudgeId = show.JudgeId,
                JudgeName = show.Judge?.UserName ?? string.Empty,
                ShowHolderId = show.ShowHolderId,
                ShowHolderName = show.ShowHolder?.Name ?? string.Empty,
                ShowType = show.ShowType.ToString(),
                ShowFormat = show.ShowFormat.ToString(),
                IsPrivate = show.IsPrivate,
                MaxEntriesPerUser = show.MaxEntriesPerUser,
                AllowMemberOnlyEntries = show.AllowMemberOnlyEntries,
                EntryDeadline = show.EntryDeadline,
                JudgingDeadline = show.JudgingDeadline,
                ResultsPublishedAt = show.ResultsPublishedAt,
                IsNanQualifying = show.IsNanQualifying,
                NamhsaGuidelines = show.NamhsaGuidelines,
                Divisions = show.Divisions.Select(d => new DivisionDto
                {
                    Id = d.Id.ToString(),
                    Name = d.Name,
                    Description = d.Description,
                    DivisionType = d.DivisionType.ToString(),
                    SortOrder = d.SortOrder,
                    ShowId = d.ShowId,
                    ShowClasses = d.ShowClasses.Select(c => new ShowClassDto
                    {
                        Id = c.Id.ToString(),
                        Name = c.Name,
                        Description = c.Description,
                        ClassNumber = c.ClassNumber,
                        MaxEntries = c.MaxEntries,
                        SortOrder = c.SortOrder,
                        BreedCategory = c.BreedCategory?.ToString(),
                        FinishType = c.FinishType?.ToString(),
                        PerformanceType = c.PerformanceType?.ToString(),
                        CollectibilityType = c.CollectibilityType?.ToString(),
                        GenderRestriction = c.GenderRestriction?.ToString(),
                        AgeRestriction = c.AgeRestriction?.ToString(),
                        ColorRestriction = c.ColorRestriction?.ToString(),
                        ScaleRestriction = c.ScaleRestriction?.ToString(),
                        DivisionId = c.DivisionId,
                        DivisionName = d.Name,
                        ShowId = c.ShowId,
                        ShowName = show.Name,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    }).ToList(),
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                }).ToList(),
                AdditionalMetadata = show.AdditionalMetadata,
                CreatedAt = show.CreatedAt,
                UpdatedAt = show.UpdatedAt
            };
        }

        private static TEnum? ParseNullable<TEnum>(string? value) where TEnum : struct
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (Enum.TryParse<TEnum>(value, true, out var e)) return e;
            return null;
        }
    }
}