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
        
        public int ShowHolderId { get; set; }
        public string ShowHolderName { get; set; } = string.Empty;
        
        // Show type and format
        public string ShowType { get; set; } = "LiveShow";
        public string ShowFormat { get; set; } = "Regular";
        
        // Privacy and access control
        public bool IsPrivate { get; set; } = false;
        
        // Entry management
        public int MaxEntriesPerUser { get; set; } = 5;
        public bool AllowMemberOnlyEntries { get; set; } = false;
        
        // Important dates
        public DateTime? EntryDeadline { get; set; }
        public DateTime? JudgingDeadline { get; set; }
        public DateTime? ResultsPublishedAt { get; set; }
        
        // Show qualification
        public bool IsNanQualifying { get; set; } = false;
        public string? NamhsaGuidelines { get; set; }
        
        // Divisions
        public List<DivisionDto> Divisions { get; set; } = new List<DivisionDto>();
        
        // Additional metadata
        public string? AdditionalMetadata { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}