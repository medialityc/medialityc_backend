using Microsoft.EntityFrameworkCore;
using Medialityc.Data.Models;

namespace Medialityc.Data;

public interface IMedialitycDbContext
{
    DbSet<Area> Areas { get; }
    DbSet<Network> Networks { get; }
    DbSet<Project> Projects { get; }
    DbSet<ReviewProject> ReviewProjects { get; }
    DbSet<RoleProfile> RoleProfiles { get; }
    DbSet<WorkProfile> WorkProfiles { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
