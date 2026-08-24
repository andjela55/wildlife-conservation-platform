using Microsoft.Extensions.DependencyInjection;
using WildlifeConservation.Services;
using WildlifeConservation.Services.Alerts;
using WildlifeConservation.Services.Animals;
using WildlifeConservation.Services.Auth;
using WildlifeConservation.Services.CollarAssignments;
using WildlifeConservation.Services.Collars;
using WildlifeConservation.Services.LocationPoints;
using WildlifeConservation.Services.RangerReports;
using WildlifeConservation.Services.Species;
using WildlifeConservation.Services.Subspecies;
using WildlifeConservation.Services.Users;

namespace WildlifeConservation.Tests;

public class ValidationServiceRegistrationTests
{
    public static TheoryData<Type, Type> Registrations => new()
    {
        { typeof(ISpeciesValidationService), typeof(SpeciesValidationService) },
        { typeof(ISubspeciesValidationService), typeof(SubspeciesValidationService) },
        { typeof(IAnimalValidationService), typeof(AnimalValidationService) },
        { typeof(IAuthValidationService), typeof(AuthValidationService) },
        { typeof(ICollarValidationService), typeof(CollarValidationService) },
        { typeof(ICollarAssignmentValidationService), typeof(CollarAssignmentValidationService) },
        { typeof(ILocationPointValidationService), typeof(LocationPointValidationService) },
        { typeof(IRangerReportValidationService), typeof(RangerReportValidationService) },
        { typeof(IAlertValidationService), typeof(AlertValidationService) },
        { typeof(IUserValidationService), typeof(UserValidationService) }
    };

    [Theory]
    [MemberData(nameof(Registrations))]
    public void AddServiceLayerRegistersScopedValidationImplementation(Type serviceType, Type implementationType)
    {
        var services = new ServiceCollection();

        services.AddServiceLayer();

        var registration = Assert.Single(services.Where(x => x.ServiceType == serviceType));
        Assert.Equal(implementationType, registration.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, registration.Lifetime);
    }
}
