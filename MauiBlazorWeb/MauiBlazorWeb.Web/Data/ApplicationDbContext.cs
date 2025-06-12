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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Show>()
                .HasOne(s => s.Judge)
                .WithMany(j => j.ShowsJudging)
                .HasForeignKey(s => s.JudgeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ShowClass>()
                .HasOne(sc => sc.Show)
                .WithMany(s => s.Classes)
                .HasForeignKey(sc => sc.ShowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Entry>()
                .HasOne(e => e.UserModelObject)
                .WithMany(h => h.Entries)
                .HasForeignKey(e => e.UserModelObjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Entry>()
                .HasOne(e => e.ShowClass)
                .WithMany(sc => sc.Entries)
                .HasForeignKey(e => e.ShowClassId);

            builder.Entity<Result>()
                .HasOne(r => r.Entry)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EntryId);

            // Configure soft delete filters
            builder.Entity<UserModelObject>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Show>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<ShowClass>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Entry>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Result>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
