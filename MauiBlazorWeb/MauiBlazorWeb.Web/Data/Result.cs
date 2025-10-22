using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiBlazorWeb.Web.Data
{
    public class Result : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        // Placement (1st, 2nd, 3rd, etc.)
        [Required]
        public int Placement { get; set; }
        
        public string Comments { get; set; } = string.Empty;
        
        [Required]
        public DateTime JudgedDate { get; set; } = DateTime.UtcNow;
        
        // Foreign key to the entry
        public int EntryId { get; set; }
        
        // Navigation property to the entry
        public virtual Entry Entry { get; set; } = null!;
    }
}