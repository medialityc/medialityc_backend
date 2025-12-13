using Microsoft.EntityFrameworkCore;
using Medialityc.Data.Models;
namespace Medialityc.Data
{

    public class MedialitycDbContext : DbContext, IMedialitycDbContext
    {
        public MedialitycDbContext(DbContextOptions<MedialitycDbContext> options) 
        : base(options){}

        public DbSet<Network> Networks  => Set<Network>();
        public DbSet<RoleProfile> RoleProfiles  =>  Set<RoleProfile>();
        public DbSet<Area> Areas => Set<Area>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ReviewProject> ReviewProjects => Set<ReviewProject>();
        public DbSet<WorkProfile> WorkProfiles => Set<WorkProfile>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReviewProject>()
                .HasOne(rp => rp.Project)
                .WithMany(p => p.Reviews)
                .HasForeignKey(rp => rp.ProjectId);

            modelBuilder.Entity<ReviewProject>()
                .HasOne(rp => rp.WorkProfile)
                .WithMany(wp => wp.Reviews)
                .HasForeignKey(rp => rp.WorkProfileId);

            base.OnModelCreating(modelBuilder);
        }
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                // Verificar que la propiedad existe antes de asignar
                if (entry.State == EntityState.Added && entry.Properties.Any(p => p.Metadata.Name == "CreatedAt"))
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
                
                if (entry.State == EntityState.Modified && entry.Properties.Any(p => p.Metadata.Name == "UpdatedAt"))
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }
            }
        }

    }
}