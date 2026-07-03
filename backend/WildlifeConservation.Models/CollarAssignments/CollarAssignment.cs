namespace WildlifeConservation.Models.CollarAssignments;

public class CollarAssignment
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public int CollarId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? UnassignedAt { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }

    public Animal? Animal { get; set; }
    public Collar? Collar { get; set; }
}
