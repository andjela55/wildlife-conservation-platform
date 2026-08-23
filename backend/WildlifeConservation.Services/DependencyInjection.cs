using Microsoft.Extensions.DependencyInjection;
using WildlifeConservation.Models;
using WildlifeConservation.Services.Alerts;
using WildlifeConservation.Services.Animals;
using WildlifeConservation.Services.Auth;
using WildlifeConservation.Services.CollarAssignments;
using WildlifeConservation.Services.Collars;
using WildlifeConservation.Services.LocationPoints;
using WildlifeConservation.Services.RangerReports;
using WildlifeConservation.Services.Species;
using WildlifeConservation.Services.Subspecies;
using WildlifeConservation.Services.Transactions;
using WildlifeConservation.Services.Users;

namespace WildlifeConservation.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(_ => { }, typeof(ModelAssemblyMarker).Assembly);

        services.AddScoped<ISpeciesService, SpeciesService>();
        services.AddScoped<ISubspeciesService, SubspeciesService>();
        services.AddScoped<IAnimalService, AnimalService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICollarService, CollarService>();
        services.AddScoped<ICollarAssignmentService, CollarAssignmentService>();
        services.AddScoped<ILocationPointService, LocationPointService>();
        services.AddScoped<IRangerReportService, RangerReportService>();
        services.AddScoped<IAlertService, AlertService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();

        return services;
    }
}
