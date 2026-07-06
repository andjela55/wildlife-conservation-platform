using Microsoft.AspNetCore.SignalR;
using WildlifeConservation.Api.DTOs.LocationPoints;
using WildlifeConservation.Api.Hubs;
using WildlifeConservation.Models.Animals;
using WildlifeConservation.Models.Collars;
using WildlifeConservation.Models.LocationPoints;
using WildlifeConservation.Services.LocationPoints;

namespace WildlifeConservation.Api.Realtime;

public class AnimalTrackingLocationPointNotificationService(IHubContext<AnimalTrackingHub> hubContext)
    : ILocationPointNotificationService
{
    public async Task NotifyLocationPointCreatedAsync(
        LocationPoint locationPoint,
        Animal animal,
        Collar collar,
        CancellationToken cancellationToken = default)
    {
        var dto = new LocationPointReceivedDto(
            locationPoint.Id,
            locationPoint.AnimalId,
            animal.Name,
            locationPoint.CollarId,
            collar.SerialNumber,
            locationPoint.Latitude,
            locationPoint.Longitude,
            locationPoint.Altitude,
            locationPoint.RecordedAt,
            locationPoint.SignalType,
            locationPoint.Notes);

        await hubContext.Clients.All.SendAsync("LocationPointReceived", dto, cancellationToken);
    }
}
