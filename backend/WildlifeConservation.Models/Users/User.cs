using WildlifeConservation.Shared.Enums;

namespace WildlifeConservation.Models.Users;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<RangerReport> RangerReports { get; set; } = new List<RangerReport>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}
