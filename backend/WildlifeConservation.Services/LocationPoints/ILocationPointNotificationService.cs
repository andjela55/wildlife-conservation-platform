namespace WildlifeConservation.Services.LocationPoints;

public interface ILocationPointNotificationService
{
    Task NotifyLocationPointCreatedAsync(
        LocationPoint locationPoint,
        Animal animal,
        Collar collar,
        CancellationToken cancellationToken = default);
}
