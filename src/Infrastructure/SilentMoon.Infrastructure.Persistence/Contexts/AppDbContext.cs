using Microsoft.EntityFrameworkCore;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;

namespace SilentMoon.Infrastructure.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Uncomment for read configurations:
            // modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Topic> Topics { get; set; }

        public DbSet<UserTopic> UserTopics { get; set; }

        public DbSet<Reminder> Reminders { get; set; }

        public DbSet<Content> Contents { get; set; }

        public DbSet<Track> Tracks { get; set; }

        public DbSet<ContentTopic> ContentTopics { get; set; }

        public DbSet<Favorite> Favorites { get; set; }

        public DbSet<PlaybackProgress> PlaybackProgresses { get; set; }
    }
}
