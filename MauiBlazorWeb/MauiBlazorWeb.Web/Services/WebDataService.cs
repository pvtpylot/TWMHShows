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

        public async Task<UserModelObjectDto> CreateUserModelObjectAsync(UserModelObjectDto userModelObjectDto)
        {
            var entity = MapToEntity(userModelObjectDto);
            var result = await _repository.CreateAsync(entity);
            return MapToDto(result);
        }

        public async Task<UserModelObjectDto> UpdateUserModelObjectAsync(string id, UserModelObjectDto userModelObjectDto)
        {
            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity == null)
            {
                return new UserModelObjectDto();
            }

            // Update properties
            existingEntity.Name = userModelObjectDto.Name;
            existingEntity.Description = userModelObjectDto.Description;
            existingEntity.Color = userModelObjectDto.Color;
            existingEntity.Size = userModelObjectDto.Size;
            existingEntity.Class = userModelObjectDto.Class;
            existingEntity.Breed = userModelObjectDto.Breed;
            existingEntity.Notes = userModelObjectDto.Notes;
            existingEntity.HeroShotImage = userModelObjectDto.HeroShotImage;

            var result = await _repository.UpdateAsync(existingEntity);
            return MapToDto(result);
        }

        public async Task<bool> DeleteUserModelObjectAsync(string id)
        {
            return await _repository.DeleteAsync(id);
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
                UpdatedAt = entity.UpdatedAt,
                HeroShotImage = entity.HeroShotImage,
                ShowImages = entity.ShowImages.Select(si => new ShowImageDto 
                { 
                    Id = si.Id, 
                    ImageData = si.ImageData 
                }).ToList()
            };
        }
        
        private static UserModelObject MapToEntity(UserModelObjectDto dto)
        {
            return new UserModelObject
            {
                Id = string.IsNullOrEmpty(dto.Id) ? Int32.Parse(Guid.NewGuid().ToString()) : int.Parse(dto.Id),
                TWEntryId = dto.TWEntryId,
                Name = dto.Name,
                Description = dto.Description,
                ApplicationUserId = dto.ApplicationUserId,
                Color = dto.Color,
                Size = dto.Size,
                Class = dto.Class,
                Breed = dto.Breed,
                Notes = dto.Notes,
                HeroShotImage = dto.HeroShotImage,
                CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt
            };
        }
    }
}