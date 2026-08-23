using Microsoft.EntityFrameworkCore;

namespace WildlifeConservation.Repositories.Data;

public class WildlifeDbContext(DbContextOptions<WildlifeDbContext> options) : DbContext(options)
{
    public DbSet<Species> Species => Set<Species>();
    public DbSet<Subspecies> Subspecies => Set<Subspecies>();
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<Collar> Collars => Set<Collar>();
    public DbSet<CollarAssignment> CollarAssignments => Set<CollarAssignment>();
    public DbSet<LocationPoint> LocationPoints => Set<LocationPoint>();
    public DbSet<RangerReport> RangerReports => Set<RangerReport>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Species).Assembly);
    }
}
