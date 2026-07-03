using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Models.Collars;

public class Collar
{
    public int Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public CollarStatus Status { get; set; } = CollarStatus.Available;
    public string? Notes { get; set; }

    public ICollection<CollarAssignment> CollarAssignments { get; set; } = new List<CollarAssignment>();
    public ICollection<LocationPoint> LocationPoints { get; set; } = new List<LocationPoint>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}
