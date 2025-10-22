using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiBlazorWeb.Web.Data;

public class Forum : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ForumType Type { get; set; }

    // For show-specific forums
    public int? ShowId { get; set; }

    [ForeignKey("ShowId")] public virtual Show? Show { get; set; }

    public bool IsActive { get; set; } = true;
    public bool RequiresAuthentication { get; set; } = true;

    public virtual ICollection<ForumPost> Posts { get; set; } = new List<ForumPost>();
}

public enum ForumType
{
    General, // General discussion
    ShowSpecific, // Specific to a show
    Authenticated, // Authenticated users only
    Public // Public access
}