using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Web.Data;
using MauiBlazorWeb.Web.Data.Repositories;
using MauiBlazorWeb.Web.Services.Mappers;

namespace MauiBlazorWeb.Web.Services
{
    public class DivisionService : MauiBlazorWeb.Shared.Services.IDivisionService
    {
        private readonly IDivisionRepository _divisionRepository;
        private readonly IEntityMapper<Division, DivisionDto> _mapper;

        public DivisionService(IDivisionRepository divisionRepository, IEntityMapper<Division, DivisionDto> mapper)
        {
            _divisionRepository = divisionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DivisionDto>> GetAllByShowIdAsync(string showId)
        {
            if (!int.TryParse(showId, out var sid) || sid <= 0)
                return Enumerable.Empty<DivisionDto>();

            var divisions = await _divisionRepository.GetAllByShowIdAsync(sid);
            return (divisions ?? new List<Division>())
                .OrderBy(d => d.SortOrder)
                .ThenBy(d => d.Name)
                .Select(_mapper.MapToDto);
        }

        public async Task<DivisionDto?> GetByIdAsync(string id)
        {
            if (!long.TryParse(id, out var did) || did <= 0 || did > int.MaxValue)
                return null;

            var division = await _divisionRepository.GetWithResultsAsync((int)did);
            return division is null ? null : _mapper.MapToDto(division);
        }

        public async Task<DivisionDto> CreateDivisionAsync(DivisionDto divisionDto)
        {
            if (divisionDto == null)
                throw new ArgumentNullException(nameof(divisionDto));

            var entity = _mapper.MapToEntity(divisionDto);
            entity.CreatedAt = DateTime.UtcNow;

            var created = await _divisionRepository.CreateAsync(entity);
            return _mapper.MapToDto(created);
        }

        public async Task<DivisionDto> UpdateDivisionAsync(DivisionDto divisionDto)
        {
            if (divisionDto == null)
                throw new ArgumentNullException(nameof(divisionDto));

            if (!long.TryParse(divisionDto.Id, out var did) || did <= 0 || did > int.MaxValue)
                throw new ArgumentException("Invalid division ID", nameof(divisionDto));

            var existing = await _divisionRepository.GetWithResultsAsync((int)did);
            if (existing == null)
                throw new KeyNotFoundException($"Division with ID {divisionDto.Id} not found");

            _mapper.MapToExistingEntity(divisionDto, existing);

            var updated = await _divisionRepository.UpdateAsync(existing);
            return _mapper.MapToDto(updated);
        }

        public async Task<bool> DeleteDivisionAsync(string id)
        {
            if (!int.TryParse(id, out var did) || did <= 0)
                return false;

            return await _divisionRepository.DeleteAsync(did);
        }
    }
}