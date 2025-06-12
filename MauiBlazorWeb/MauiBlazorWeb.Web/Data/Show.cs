using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiBlazorWeb.Web.Data
{
    public class Show : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime ShowDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [Required]
        public ShowStatus Status { get; set; } = ShowStatus.Upcoming;
        
        // Foreign key to the judge
        public string? JudgeId { get; set; }
        
        // Navigation property to the judge
        public virtual ApplicationUser? Judge { get; set; }
        
        // Navigation properties
        public virtual ICollection<ShowClass> Classes { get; set; } = new List<ShowClass>();
    }
    
    public enum ShowStatus
    {
        Upcoming,
        InProgress,
        Completed,
        Cancelled
    }
}