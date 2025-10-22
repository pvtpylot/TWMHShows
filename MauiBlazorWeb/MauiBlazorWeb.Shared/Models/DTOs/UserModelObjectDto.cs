using System.ComponentModel.DataAnnotations;

namespace MauiBlazorWeb.Shared.Models.DTOs;

public class UserModelObjectDto
{
    public string Id { get; set; } = string.Empty;
    public int? TWEntryId { get; set; }

    [Required] public string Name { get; set; } = string.Empty;

    [Required] public string Description { get; set; } = string.Empty;

    public string? ApplicationUserId { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string Breed { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Store the hero shot image as a byte array
    public byte[]? HeroShotImage { get; set; }

    // Store show images as a list of byte arrays
    public List<ShowImageDto> ShowImages { get; set; } = new();
}

public class ShowImageDto
{
    public int Id { get; set; }
    public byte[] ImageData { get; set; } = [];
}