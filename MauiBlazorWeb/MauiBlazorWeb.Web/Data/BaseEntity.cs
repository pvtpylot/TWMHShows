namespace MauiBlazorWeb.Web.Data;

/// <summary>
///     Base class for all entities to enforce common properties
/// </summary>
public abstract class BaseEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}