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
        
        
        // One-to-many relationship with Division
        public ICollection<Division> Divisions { get; set; } = new List<Division>();
        public int ShowHolderId { get; set; }
        [ForeignKey("ShowHolderId")]
        public UserModelObject? ShowHolder { get; set; }
        
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EntryDeadline { get; set; }
        public DateTime? JudgingDeadline { get; set; }
        public DateTime? ResultsPublishedAt { get; set; }
        public int MaxEntriesPerUser { get; set; } = 5;
    }
    
    public enum ShowStatus
    {
        Upcoming,
        InProgress,
        Completed,
        Cancelled
    }
}