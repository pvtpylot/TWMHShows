using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiBlazorWeb.Web.Data
{
    public class ForumPost : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
        
        // Author (can be null for anonymous posts)
        public string? AuthorId { get; set; }
        
        [ForeignKey("AuthorId")]
        public virtual ApplicationUser? Author { get; set; }
        
        // For anonymous posts
        public string? AnonymousName { get; set; }
        
        // Forum relationship
        public int ForumId { get; set; }
        
        [ForeignKey("ForumId")]
        public virtual Forum Forum { get; set; } = null!;
        
        // Parent post for replies
        public int? ParentPostId { get; set; }
        
        [ForeignKey("ParentPostId")]
        public virtual ForumPost? ParentPost { get; set; }
        
        public virtual ICollection<ForumPost> Replies { get; set; } = new List<ForumPost>();
        
        public bool IsPinned { get; set; } = false;
        public bool IsModerated { get; set; } = false;
    }
}