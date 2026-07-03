using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Models.Alerts;

public class Alert
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public int? CollarId { get; set; }
    public int? CreatedByUserId { get; set; }
    public AlertType AlertType { get; set; }
    public Severity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public Animal? Animal { get; set; }
    public Collar? Collar { get; set; }
    public User? CreatedByUser { get; set; }
}
