using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiBlazorWeb.Web.Data
{
    public class LiveShow : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int ShowId { get; set; }
        
        [ForeignKey("ShowId")]
        public virtual Show Show { get; set; } = null!;
        
        public bool IsLive { get; set; } = false;
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        
        // Current class being judged
        public int? CurrentClassId { get; set; }
        
        [ForeignKey("CurrentClassId")]
        public virtual ShowClass? CurrentClass { get; set; }
        
        public virtual ICollection<LiveComment> Comments { get; set; } = new List<LiveComment>();
        public virtual ICollection<LiveAnnouncement> Announcements { get; set; } = new List<LiveAnnouncement>();
    }
}