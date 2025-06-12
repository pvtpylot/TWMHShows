using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiBlazorWeb.Web.Data
{
    public class ShowClass : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public int ClassNumber { get; set; }
        
        public int? MaxEntries { get; set; }
        
        // Foreign key to the Show
        public int ShowId { get; set; }
        
        // Navigation property to the Show
        public virtual Show Show { get; set; } = null!;
        
        // Navigation properties
        public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
    }
}