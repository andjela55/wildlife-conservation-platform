using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Models.RangerReports;

public class RangerReport
{
    public int Id { get; set; }
    public int? AnimalId { get; set; }
    public int UserId { get; set; }
    public ReportType ReportType { get; set; }
    public Severity Severity { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Animal? Animal { get; set; }
    public User? User { get; set; }
}
