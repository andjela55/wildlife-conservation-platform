using System.Text.Json.Serialization;
using WildlifeConservation.Api.Realtime;
using WildlifeConservation.Services.LocationPoints;

namespace WildlifeConservation.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiLayer(this IServiceCollection services)
    {
        services.AddSignalR()
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddScoped<ILocationPointNotificationService, AnimalTrackingLocationPointNotificationService>();

        return services;
    }
}
