using System.ComponentModel.DataAnnotations;

namespace MauiBlazorWeb.Shared.Models.DTOs;

public class ShowClassDto
{
    public string Id { get; set; } = string.Empty;

    [Required] public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required] public int ClassNumber { get; set; }

    public int? MaxEntries { get; set; } = 3;

    public int SortOrder { get; set; } = 0;

    // Class categorization
    public string? BreedCategory { get; set; }
    public string? FinishType { get; set; }
    public string? PerformanceType { get; set; }
    public string? CollectibilityType { get; set; }

    // Additional class specifications
    public string? GenderRestriction { get; set; }
    public string? AgeRestriction { get; set; }
    public string? ColorRestriction { get; set; }
    public string? ScaleRestriction { get; set; }

    public long DivisionId { get; set; }
    public string DivisionName { get; set; } = string.Empty;

    public int ShowId { get; set; }
    public string ShowName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}