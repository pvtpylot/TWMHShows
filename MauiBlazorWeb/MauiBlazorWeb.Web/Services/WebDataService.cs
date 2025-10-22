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
    public class WebDataService : IDataService
    {
        private readonly IUserModelObjectRepository _repository;
        private readonly IEntityMapper<UserModelObject, UserModelObjectDto> _mapper;

        public WebDataService(
            IUserModelObjectRepository repository,
            IEntityMapper<UserModelObject, UserModelObjectDto> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserModelObjectDto>> GetAllUserModelObjectsAsync(string? applicationUserId)
        {
            var entities = await _repository.GetAllAsync(applicationUserId);
            return entities.Select(_mapper.MapToDto);
        }

        public async Task<UserModelObjectDto> GetUserModelObjectByIdAsync(string id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? _mapper.MapToDto(entity) : new UserModelObjectDto();
        }

        public async Task<UserModelObjectDto> CreateUserModelObjectAsync(UserModelObjectDto userModelObjectDto)
        {
            var entity = _mapper.MapToEntity(userModelObjectDto);
            var result = await _repository.CreateAsync(entity);
            return _mapper.MapToDto(result);
        }

        public async Task<UserModelObjectDto> UpdateUserModelObjectAsync(string id, UserModelObjectDto userModelObjectDto)
        {
            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity == null)
            {
                return new UserModelObjectDto();
            }

            // Update entity properties by mapping from DTO
            _mapper.MapToExistingEntity(userModelObjectDto, existingEntity);

            var result = await _repository.UpdateAsync(existingEntity);
            return _mapper.MapToDto(result);
        }

        public async Task<bool> DeleteUserModelObjectAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}