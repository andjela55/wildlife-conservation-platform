using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Models.Animals;

public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SubspeciesId { get; set; }
    public AnimalSex Sex { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    public Models.Subspecies.Subspecies? Subspecies { get; set; }
    public ICollection<CollarAssignment> CollarAssignments { get; set; } = new List<CollarAssignment>();
    public ICollection<LocationPoint> LocationPoints { get; set; } = new List<LocationPoint>();
    public ICollection<RangerReport> RangerReports { get; set; } = new List<RangerReport>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}
