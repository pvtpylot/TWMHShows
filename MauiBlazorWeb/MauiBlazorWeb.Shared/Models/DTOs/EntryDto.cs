using System;
using System.ComponentModel.DataAnnotations;

namespace MauiBlazorWeb.Shared.Models.DTOs
{
    public class EntryDto
    {
        public string Id { get; set; } = string.Empty;
        
        [Required]
        public int EntryNumber { get; set; }
        
        [Required]
        public DateTime SubmissionDate { get; set; }
        
        [Required]
        public string Status { get; set; } = "Submitted";
        
        [Required]
        public string UserModelObjectId { get; set; } = string.Empty;
        
        public string HorseName { get; set; } = string.Empty;
        
        [Required]
        public string ShowClassId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}