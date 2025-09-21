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
    public class ShowClassService : IShowClassService
    {
        private readonly IShowClassRepository _showClassRepository;

        public ShowClassService(IShowClassRepository showClassRepository)
        {
            _showClassRepository = showClassRepository;
        }

        public async Task<IEnumerable<ShowClassDto>> GetClassesByShowIdAsync(string showId)
        {
            var classes = await _showClassRepository.GetAllByShowIdAsync(showId);
            return classes.Select(MapToDto);
        }

        public async Task<ShowClassDto?> GetShowClassByIdAsync(string id)
        {
            var showClass = await _showClassRepository.GetByIdAsync(id);
            return showClass != null ? MapToDto(showClass) : null;
        }

        public async Task<ShowClassDto> CreateShowClassAsync(ShowClassDto showClassDto)
        {
            // if (!int.TryParse(showClassDto.ShowId, out var showId))
            //     throw new ArgumentException("Invalid show ID");
            var showId = showClassDto.ShowId;
                
            var showClass = new ShowClass
            {
                Name = showClassDto.Name,
                Description = showClassDto.Description,
                ClassNumber = showClassDto.ClassNumber,
                MaxEntries = showClassDto.MaxEntries,
                ShowId = showId
            };

            var result = await _showClassRepository.CreateAsync(showClass);
            return MapToDto(result);
        }

        public async Task<ShowClassDto> UpdateShowClassAsync(ShowClassDto showClassDto)
        {
            if (!int.TryParse(showClassDto.Id, out var classId))
                throw new ArgumentException("Invalid class ID");
                
            // if (!int.TryParse(showClassDto.ShowId, out var showId))
            //     throw new ArgumentException("Invalid show ID");
            var showId = showClassDto.ShowId;
            var showClass = await _showClassRepository.GetByIdAsync(showClassDto.Id);
            if (showClass == null)
                throw new KeyNotFoundException($"Show class with ID {showClassDto.Id} not found");

            showClass.Name = showClassDto.Name;
            showClass.Description = showClassDto.Description;
            showClass.ClassNumber = showClassDto.ClassNumber;
            showClass.MaxEntries = showClassDto.MaxEntries;
            showClass.ShowId = showId;

            var result = await _showClassRepository.UpdateAsync(showClass);
            return MapToDto(result);
        }

        public async Task<bool> DeleteShowClassAsync(string id)
        {
            return await _showClassRepository.DeleteAsync(id);
        }

        private static ShowClassDto MapToDto(ShowClass showClass)
        {
            return new ShowClassDto
            {
                Id = showClass.Id.ToString(),
                Name = showClass.Name,
                Description = showClass.Description,
                ClassNumber = showClass.ClassNumber,
                MaxEntries = showClass.MaxEntries,
                ShowId = showClass.ShowId,
                CreatedAt = showClass.CreatedAt,
                UpdatedAt = showClass.UpdatedAt
            };
        }
    }
}