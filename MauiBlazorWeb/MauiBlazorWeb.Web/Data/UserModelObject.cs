using System.ComponentModel.DataAnnotations;

namespace MauiBlazorWeb.Web.Data;

public class UserModelObject : BaseEntity
{
    [Key] public int Id { get; set; }

    public int? TWEntryId { get; set; }

    [Required] public string Name { get; set; } = string.Empty;

    [Required] public string Description { get; set; } = string.Empty;

    public string? ApplicationUserId { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string Breed { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    // Store the hero shot image as a byte array
    public byte[]? HeroShotImage { get; set; }

    // Store multiple show images as byte arrays
    public virtual ICollection<ShowImage> ShowImages { get; set; } = new List<ShowImage>();

    // Navigation property to entries
    public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
}

public class ShowImage
{
    [Key] public int Id { get; set; }

    public byte[] ImageData { get; set; } = [];
    public int UserModelObjectId { get; set; }
    public virtual UserModelObject UserModelObject { get; set; } = null!;
}