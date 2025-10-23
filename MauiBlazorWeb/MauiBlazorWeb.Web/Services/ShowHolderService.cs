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
    public class ShowHolderService : IShowHolderService
    {
        private readonly IShowRepository _showRepository;
        private readonly IUserModelObjectRepository _umoRepository;

        public ShowHolderService(IShowRepository showRepository, IUserModelObjectRepository umoRepository)
        {
            _showRepository = showRepository;
            _umoRepository = umoRepository;
        }

        public async Task<IEnumerable<ShowDto>> GetMyShowsAsync(string userId, bool isAdmin = false)
        {
            if (isAdmin)
            {
                var all = await _showRepository.GetAllAsync();
                return all.Select(MapToDto);
            }

            var mine = await _showRepository.GetAllByShowHolderUserIdAsync(userId);
            return mine.Select(MapToDto);
        }

        public async Task<ShowDto> CreateShowAsync(string userId, ShowDto showDto, bool isAdmin = false)
        {
            await EnsureShowHolderOwnershipAsync(userId, showDto.ShowHolderId, isAdmin);

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
                AdditionalMetadata = showDto.AdditionalMetadata
            };

            var created = await _showRepository.CreateAsync(show);
            return MapToDto(created);
        }

        public async Task<ShowDto> UpdateShowAsync(string userId, ShowDto showDto, bool isAdmin = false)
        {
            if (string.IsNullOrWhiteSpace(showDto.Id))
                throw new ArgumentException("Show ID is required", nameof(showDto));

            var existing = await _showRepository.GetByIdWithShowHolderAsync(showDto.Id)
                           ?? throw new KeyNotFoundException($"Show {showDto.Id} not found");

            if (!isAdmin)
            {
                EnsureOwnership(userId, existing);
            }

            // Validate potential change of ShowHolderId
            if (existing.ShowHolderId != showDto.ShowHolderId)
            {
                await EnsureShowHolderOwnershipAsync(userId, showDto.ShowHolderId, isAdmin);
            }

            existing.Name = showDto.Name;
            existing.Description = showDto.Description;
            existing.ShowDate = showDto.ShowDate;
            existing.EndDate = showDto.EndDate;
            existing.Status = Enum.Parse<ShowStatus>(showDto.Status, ignoreCase: true);
            existing.JudgeId = showDto.JudgeId;
            existing.ShowHolderId = showDto.ShowHolderId;
            existing.ShowType = Enum.Parse<ShowType>(showDto.ShowType, ignoreCase: true);
            existing.ShowFormat = Enum.Parse<ShowFormat>(showDto.ShowFormat, ignoreCase: true);
            existing.IsPrivate = showDto.IsPrivate;
            existing.MaxEntriesPerUser = showDto.MaxEntriesPerUser;
            existing.AllowMemberOnlyEntries = showDto.AllowMemberOnlyEntries;
            existing.EntryDeadline = showDto.EntryDeadline;
            existing.JudgingDeadline = showDto.JudgingDeadline;
            existing.ResultsPublishedAt = showDto.ResultsPublishedAt;
            existing.IsNanQualifying = showDto.IsNanQualifying;
            existing.NamhsaGuidelines = showDto.NamhsaGuidelines;
            existing.AdditionalMetadata = showDto.AdditionalMetadata;

            var updated = await _showRepository.UpdateAsync(existing);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteShowAsync(string userId, string showId, bool isAdmin = false)
        {
            var existing = await _showRepository.GetByIdWithShowHolderAsync(showId);
            if (existing == null) return false;

            if (!isAdmin)
            {
                EnsureOwnership(userId, existing);
            }

            return await _showRepository.DeleteAsync(showId);
        }

        private async Task EnsureShowHolderOwnershipAsync(string userId, int showHolderId, bool isAdmin)
        {
            if (isAdmin) return;

            var umo = await _umoRepository.GetByIdAsync(showHolderId.ToString());
            if (umo == null || !string.Equals(umo.ApplicationUserId, userId, StringComparison.Ordinal))
            {
                throw new UnauthorizedAccessException("ShowHolderId does not belong to the current user.");
            }
        }

        private static void EnsureOwnership(string userId, Show show)
        {
            var ownerId = show.ShowHolder?.ApplicationUserId;
            if (!string.Equals(ownerId, userId, StringComparison.Ordinal))
            {
                throw new UnauthorizedAccessException("You do not own this show.");
            }
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
                AdditionalMetadata = show.AdditionalMetadata,
                CreatedAt = show.CreatedAt,
                UpdatedAt = show.UpdatedAt
            };
        }
    }
}