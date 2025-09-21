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
        
        // Show holder information
        public int ShowHolderId { get; set; }
        [ForeignKey("ShowHolderId")]
        public UserModelObject? ShowHolder { get; set; }
        
        // Show type and format
        [Required]
        public ShowType ShowType { get; set; } = ShowType.LiveShow;
        
        [Required]
        public ShowFormat ShowFormat { get; set; } = ShowFormat.Regular;
        
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
        
        // One-to-many relationship with Division
        public ICollection<Division> Divisions { get; set; } = new List<Division>();
        
        // Additional metadata storage for future requirements
        public string? AdditionalMetadata { get; set; } // JSON string for flexible metadata
        
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public enum ShowStatus
    {
        Upcoming,
        InProgress,
        Completed,
        Cancelled
    }
    
    public enum ShowType
    {
        LiveShow,
        PhotoShow
    }
    
    public enum ShowFormat
    {
        Regular,
        NanQualifying,
        Championship,
        Specialty
    }
}