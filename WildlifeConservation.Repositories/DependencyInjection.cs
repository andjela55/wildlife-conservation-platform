using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WildlifeConservation.Repositories.Data;
using WildlifeConservation.Repositories.Repositories.Alerts;
using WildlifeConservation.Repositories.Repositories.Animals;
using WildlifeConservation.Repositories.Repositories.CollarAssignments;
using WildlifeConservation.Repositories.Repositories.Collars;
using WildlifeConservation.Repositories.Repositories.LocationPoints;
using WildlifeConservation.Repositories.Repositories.RangerReports;
using WildlifeConservation.Repositories.Repositories.Species;
using WildlifeConservation.Repositories.Repositories.Subspecies;
using WildlifeConservation.Repositories.Repositories.Users;

namespace WildlifeConservation.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositoryLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WildlifeDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly(typeof(WildlifeDbContext).Assembly.FullName)));

        services.AddScoped<ISpeciesRepository, SpeciesRepository>();
        services.AddScoped<ISubspeciesRepository, SubspeciesRepository>();
        services.AddScoped<IAnimalRepository, AnimalRepository>();
        services.AddScoped<ICollarRepository, CollarRepository>();
        services.AddScoped<ICollarAssignmentRepository, CollarAssignmentRepository>();
        services.AddScoped<ILocationPointRepository, LocationPointRepository>();
        services.AddScoped<IRangerReportRepository, RangerReportRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
