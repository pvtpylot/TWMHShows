using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MauiBlazorWeb.Shared.Models.DTOs
{
    public class DivisionDto
    {
        public string Id { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public string DivisionType { get; set; } = "Halter";
        
        public int SortOrder { get; set; } = 0;
        
        public int ShowId { get; set; }
        
        public List<ShowClassDto> ShowClasses { get; set; } = new List<ShowClassDto>();
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}