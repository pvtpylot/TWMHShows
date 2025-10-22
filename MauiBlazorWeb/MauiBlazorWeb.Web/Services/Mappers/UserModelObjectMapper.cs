using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Web.Data;

namespace MauiBlazorWeb.Web.Services.Mappers;

public class UserModelObjectMapper : IEntityMapper<UserModelObject, UserModelObjectDto>
{
    public UserModelObjectDto MapToDto(UserModelObject entity)
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

    public UserModelObject MapToEntity(UserModelObjectDto dto)
    {
        return new UserModelObject
        {
            Id = string.IsNullOrEmpty(dto.Id) ? 0 : int.TryParse(dto.Id, out var id) ? id : 0,
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

    public void MapToExistingEntity(UserModelObjectDto dto, UserModelObject existingEntity)
    {
        // Update only the properties that should be updated
        existingEntity.Name = dto.Name;
        existingEntity.Description = dto.Description;
        existingEntity.Color = dto.Color;
        existingEntity.Size = dto.Size;
        existingEntity.Class = dto.Class;
        existingEntity.Breed = dto.Breed;
        existingEntity.Notes = dto.Notes;
        existingEntity.HeroShotImage = dto.HeroShotImage;

        // Don't update:
        // - Id (primary key)
        // - CreatedAt (creation timestamp)
        // - ApplicationUserId (ownership)
    }
}