using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Models.LocationPoints;

public class LocationPoint
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public int CollarId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal? Altitude { get; set; }
    public DateTime RecordedAt { get; set; }
    public SignalType SignalType { get; set; }
    public string? Notes { get; set; }

    public Animal? Animal { get; set; }
    public Collar? Collar { get; set; }
}
