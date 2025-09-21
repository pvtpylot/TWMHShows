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

        public int? MaxEntries { get; set; } = 3;
        
        // Display order within the division
        public int SortOrder { get; set; } = 0;
        
        // Class categorization based on model horse show standards
        public BreedCategory? BreedCategory { get; set; }
        public FinishType? FinishType { get; set; }
        public PerformanceType? PerformanceType { get; set; }
        public CollectibilityType? CollectibilityType { get; set; }
        
        // Additional class specifications
        public Gender? GenderRestriction { get; set; }
        public AgeCategory? AgeRestriction { get; set; }
        public ColorRestriction? ColorRestriction { get; set; }
        public ScaleCategory? ScaleRestriction { get; set; }
        
        // Foreign key for Division (many-to-one)
        public long DivisionId { get; set; }
    
        // Navigation property to Division
        [ForeignKey("DivisionId")]
        public Division? Division { get; set; }
        
        // Foreign key to the Show
        public int ShowId { get; set; }
        
        // Navigation property to the Show
        [ForeignKey("ShowId")]
        public Show Show { get; set; } = null!;
        
        // Navigation properties
        public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
    }
    
    // Enums for class categorization based on model horse show standards
    public enum BreedCategory
    {
        SportBreeds,     // Thoroughbred, Warmbloods, Standardbred, Akhal-Teke
        LightBreeds,     // Arabian, Morgan, Saddlebred, Tennessee Walking Horse
        DraftBreeds,     // Clydesdale, Percheron, Shire, Belgian
        PonyBreeds,      // Welsh, Shetland, Connemara, POA, Hackney
        StockBreeds,     // Quarter Horse, Paint, Appaloosa, Mustang
        OtherBreeds      // Mixed, rare, or exotic breeds
    }
    
    public enum FinishType
    {
        OriginalFinish,  // Factory-produced models
        Custom,          // Modified models
        ArtistResin      // Artist-created pieces
    }
    
    public enum PerformanceType
    {
        // English Performance
        Hunter,
        Jumper,
        Dressage,
        EnglishPleasure,
        EnglishTrail,
        
        // Western Performance
        Reining,
        WesternPleasure,
        BarrelRacing,
        WesternTrail,
        
        // Other Performance
        Costume,
        Parade,
        Harness,
        Scene,
        Setup
    }
    
    public enum CollectibilityType
    {
        BreyerModels,
        StoneModels,
        HagenRenaker,
        VintageModels,
        LimitedEdition,
        TestColors,
        OtherCollectible
    }
    
    public enum Gender
    {
        Stallion,
        Mare,
        Gelding,
        Foal
    }
    
    public enum AgeCategory
    {
        Foal,
        Yearling,
        Adult,
        Any
    }
    
    public enum ColorRestriction
    {
        Bay,
        Chestnut,
        Black,
        Gray,
        Palomino,
        Buckskin,
        Pinto,
        Appaloosa,
        Any
    }
    
    public enum ScaleCategory
    {
        Traditional,     // 1:9 scale (Breyer Traditional)
        Classic,         // 1:12 scale (Breyer Classic)
        Stablemate,      // 1:32 scale (Breyer Stablemate)
        Micro,          // Very small scale
        Any
    }
}