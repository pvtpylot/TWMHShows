using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;
using MauiBlazorWeb.Web.Data;
using MauiBlazorWeb.Web.Data.Repositories;
using MauiBlazorWeb.Web.Services.Mappers;

namespace MauiBlazorWeb.Web.Services
{
    public class ShowService : IShowService
    {
        private readonly IShowRepository _showRepository;
        private readonly IEntityMapper<Show, ShowDto> _mapper;

        public ShowService(IShowRepository showRepository, IEntityMapper<Show, ShowDto> mapper)
        {
            _showRepository = showRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ShowDto>> GetAllShowsAsync()
        {
            var shows = await _showRepository.GetAllAsync();
            return shows.Select(_mapper.MapToDto);
        }

        public async Task<IEnumerable<ShowDto>> GetShowsByJudgeIdAsync(string judgeId)
        {
            var shows = await _showRepository.GetAllByJudgeIdAsync(judgeId);
            return shows.Select(_mapper.MapToDto);
        }

        public async Task<ShowDto?> GetShowByIdAsync(string id)
        {
            var show = await _showRepository.GetByIdAsync(id);
            return show != null ? _mapper.MapToDto(show) : null;
        }

        public async Task<ShowDto> CreateShowAsync(ShowDto showDto)
        {
            var show = _mapper.MapToEntity(showDto);
            var result = await _showRepository.CreateAsync(show);
            return _mapper.MapToDto(result);
        }

        public async Task<ShowDto> UpdateShowAsync(ShowDto showDto)
        {
            if (!int.TryParse(showDto.Id, out var _))
                throw new ArgumentException("Invalid show ID");

            var show = await _showRepository.GetByIdAsync(showDto.Id);
            if (show == null)
                throw new KeyNotFoundException($"Show with ID {showDto.Id} not found");

            _mapper.MapToExistingEntity(showDto, show);
            var result = await _showRepository.UpdateAsync(show);
            return _mapper.MapToDto(result);
        }

        public async Task<bool> DeleteShowAsync(string id)
        {
            return await _showRepository.DeleteAsync(id);
        }
    }
}