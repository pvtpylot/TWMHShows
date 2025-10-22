using System.ComponentModel.DataAnnotations;

namespace MauiBlazorWeb.Shared.Models.DTOs;

public class ResultDto
{
    public string Id { get; set; } = string.Empty;

    [Required] public int Placement { get; set; }

    public string Comments { get; set; } = string.Empty;

    [Required] public DateTime JudgedDate { get; set; }

    [Required] public string EntryId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}