using System.ComponentModel.DataAnnotations;

namespace MauiBlazorWeb.Web.Data.DTOs
{
    public class UserModelObjectDto : BaseDto
    {
        [Key]
        public int Id { get; set; }
        public int? TWEntryId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; } = null!;
        public string Color { get; set; }
        public string Size { get; set; }
        public string Class { get; set; }
        public string Breed { get; set; }
        public string Notes { get; set; }
    }
}
