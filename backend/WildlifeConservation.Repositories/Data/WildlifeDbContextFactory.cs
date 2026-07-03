using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WildlifeConservation.Repositories.Data;

public class WildlifeDbContextFactory : IDesignTimeDbContextFactory<WildlifeDbContext>
{
    public WildlifeDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("WILDLIFE_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=wildlife_conservation;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<WildlifeDbContext>();
        optionsBuilder.UseNpgsql(
            connectionString,
            npgsql => npgsql.MigrationsAssembly(typeof(WildlifeDbContext).Assembly.FullName));

        return new WildlifeDbContext(optionsBuilder.Options);
    }
}
