using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MauiBlazorWeb.Shared.Models.DTOs
{
    public class ShowDto
    {
        public string Id { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime ShowDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [Required]
        public string Status { get; set; } = "Upcoming";
        
        public string? JudgeId { get; set; }
        
        public string JudgeName { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}