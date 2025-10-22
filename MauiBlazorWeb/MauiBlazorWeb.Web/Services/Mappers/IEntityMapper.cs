namespace MauiBlazorWeb.Web.Services.Mappers;

/// <summary>
///     Interface for mapping between entity and DTO objects
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TDto">The DTO type</typeparam>
public interface IEntityMapper<TEntity, TDto>
{
    /// <summary>
    ///     Maps an entity to a DTO
    /// </summary>
    /// <param name="entity">The entity to map</param>
    /// <returns>A new DTO instance</returns>
    TDto MapToDto(TEntity entity);

    /// <summary>
    ///     Maps a DTO to a new entity
    /// </summary>
    /// <param name="dto">The DTO to map</param>
    /// <returns>A new entity instance</returns>
    TEntity MapToEntity(TDto dto);

    /// <summary>
    ///     Maps a DTO to an existing entity instance
    /// </summary>
    /// <param name="dto">The source DTO</param>
    /// <param name="existingEntity">The existing entity to update</param>
    void MapToExistingEntity(TDto dto, TEntity existingEntity);
}