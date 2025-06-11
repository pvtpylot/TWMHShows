using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;
using MauiBlazorWeb.Web.Data;
using MauiBlazorWeb.Web.Data.Repositories;

namespace MauiBlazorWeb.Web.Services
{
    public class WebDataService : IDataService
    {
        private readonly IUserModelObjectRepository _repository;

        public WebDataService(IUserModelObjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<UserModelObjectDto>> GetAllUserModelObjectsAsync(string? applicationUserId)
        {
            var entities = await _repository.GetAllAsync(applicationUserId);
            return entities.Select(MapToDto);
        }

        public async Task<UserModelObjectDto> GetUserModelObjectByIdAsync(string id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? MapToDto(entity) : new UserModelObjectDto();
        }

        private static UserModelObjectDto MapToDto(UserModelObject entity)
        {
            return new UserModelObjectDto
            {
                Id = entity.Id.ToString(),
                TWEntryId = entity.TWEntryId,
                Name = entity.Name,
                Description = entity.Description,
                ApplicationUserId = entity.ApplicationUserId,
                Color = entity.Color ?? string.Empty,
                Size = entity.Size ?? string.Empty,
                Class = entity.Class ?? string.Empty,
                Breed = entity.Breed ?? string.Empty,
                Notes = entity.Notes ?? string.Empty,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}