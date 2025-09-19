using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiBlazorWeb.Web.Data
{
    public class LiveAnnouncement : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
        
        public AnnouncementPriority Priority { get; set; } = AnnouncementPriority.Normal;
        
        // Author (ShowHolder or Admin)
        public string AuthorId { get; set; } = string.Empty;
        
        [ForeignKey("AuthorId")]
        public virtual ApplicationUser Author { get; set; } = null!;
        
        // Live show relationship
        public int LiveShowId { get; set; }
        
        [ForeignKey("LiveShowId")]
        public virtual LiveShow LiveShow { get; set; } = null!;
    }
    
    public enum AnnouncementPriority
    {
        Low,
        Normal,
        High,
        Critical
    }
}