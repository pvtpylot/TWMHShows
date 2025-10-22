using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiBlazorWeb.Web.Data;

public class Entry : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public int EntryNumber { get; set; }

    [Required] public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;

    [Required] public EntryStatus Status { get; set; } = EntryStatus.Submitted;

    // Foreign key to the user's horse (UserModelObject)
    public int UserModelObjectId { get; set; }

    // Navigation property to the user's horse
    public virtual UserModelObject UserModelObject { get; set; } = null!;

    // Foreign key to the show class
    public int ShowClassId { get; set; }

    // Navigation property to the show class
    public virtual ShowClass ShowClass { get; set; } = null!;

    // Navigation property to result
    public virtual Result? Result { get; set; }
}

public enum EntryStatus
{
    Submitted,
    Accepted,
    Rejected
}