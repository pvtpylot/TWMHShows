using System;
using System.ComponentModel.DataAnnotations;

namespace MauiBlazorWeb.Shared.Models.DTOs
{
    public class ShowClassDto
    {
        public string Id { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public int ClassNumber { get; set; }
        
        public int? MaxEntries { get; set; }
        
        [Required]
        public string ShowId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}