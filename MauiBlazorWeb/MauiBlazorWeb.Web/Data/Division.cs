using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiBlazorWeb.Web.Data;

public class Division : BaseEntity
{
    [Key]
    public long Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    
    // Many-to-one relationship with Show
    public int ShowId { get; set; }
    
    [ForeignKey("ShowId")]
    public Show? Show { get; set; }
    
    // One-to-many relationship with ShowClass
    public ICollection<ShowClass> ShowClasses { get; set; } = new List<ShowClass>();
}