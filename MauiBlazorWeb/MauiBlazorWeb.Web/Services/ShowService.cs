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
                Status = Enum.Parse<ShowStatus>(showDto.Status),
                JudgeId = showDto.JudgeId
            };

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
            show.Status = Enum.Parse<ShowStatus>(showDto.Status);
            show.JudgeId = showDto.JudgeId;

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
                CreatedAt = show.CreatedAt,
                UpdatedAt = show.UpdatedAt
            };
        }
    }
}