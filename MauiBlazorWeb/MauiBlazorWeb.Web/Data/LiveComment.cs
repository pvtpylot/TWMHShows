using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiBlazorWeb.Web.Data
{
    public class LiveComment : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
        
        public CommentType Type { get; set; } = CommentType.Chat;
        
        // Author
        public string? AuthorId { get; set; }
        
        [ForeignKey("AuthorId")]
        public virtual ApplicationUser? Author { get; set; }
        
        // Live show relationship
        public int LiveShowId { get; set; }
        
        [ForeignKey("LiveShowId")]
        public virtual LiveShow LiveShow { get; set; } = null!;
        
        // Class-specific comments
        public int? ShowClassId { get; set; }
        
        [ForeignKey("ShowClassId")]
        public virtual ShowClass? ShowClass { get; set; }
    }
    
    public enum CommentType
    {
        Chat,
        JudgeCommentary,
        Announcement
    }
}