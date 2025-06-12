using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MauiBlazorWeb.Web.Data
{
    public class UserModelObject : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int? TWEntryId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public string? ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        
        // Navigation property to entries
        public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
    }
}
