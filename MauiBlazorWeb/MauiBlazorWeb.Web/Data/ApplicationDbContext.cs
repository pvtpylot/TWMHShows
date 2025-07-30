using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MauiBlazorWeb.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        public DbSet<UserModelObject> UserModelObjects { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<ShowClass> ShowClasses { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Division> Divisions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Show>()
                .HasMany(s => s.Divisions)
                .WithOne(d => d.Show)
                .HasForeignKey(d => d.ShowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Division>()
                .HasMany(d => d.ShowClasses)
                .WithOne(sc => sc.Division)
                .HasForeignKey(sc => sc.DivisionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ShowClass>()
                .HasOne(sc => sc.Division)
                .WithMany(d => d.ShowClasses)
                .HasForeignKey(sc => sc.DivisionId);

            // Configure soft delete filters
            builder.Entity<UserModelObject>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Show>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Division>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<ShowClass>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Entry>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Result>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
