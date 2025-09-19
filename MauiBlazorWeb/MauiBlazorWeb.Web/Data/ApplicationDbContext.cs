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
        public DbSet<Forum> Forums { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<LiveShow> LiveShows { get; set; }
        public DbSet<LiveComment> LiveComments { get; set; }
        public DbSet<LiveAnnouncement> LiveAnnouncements { get; set; }

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

            // Forum relationships
            builder.Entity<Forum>()
                .HasMany(f => f.Posts)
                .WithOne(p => p.Forum)
                .HasForeignKey(p => p.ForumId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ForumPost>()
                .HasMany(p => p.Replies)
                .WithOne(r => r.ParentPost)
                .HasForeignKey(r => r.ParentPostId)
                .OnDelete(DeleteBehavior.Restrict);

            // Live show relationships
            builder.Entity<LiveShow>()
                .HasMany(ls => ls.Comments)
                .WithOne(c => c.LiveShow)
                .HasForeignKey(c => c.LiveShowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LiveShow>()
                .HasMany(ls => ls.Announcements)
                .WithOne(a => a.LiveShow)
                .HasForeignKey(a => a.LiveShowId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure soft delete filters
            builder.Entity<UserModelObject>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Show>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Division>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<ShowClass>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Entry>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Result>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Forum>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<ForumPost>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<LiveShow>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<LiveComment>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<LiveAnnouncement>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
