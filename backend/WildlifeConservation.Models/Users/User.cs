namespace WildlifeConservation.Models.Users;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string? AssignedLocationName { get; set; }
    public decimal? AssignedLatitude { get; set; }
    public decimal? AssignedLongitude { get; set; }
    public int? AssignedMapZoom { get; set; }

    public ICollection<RangerReport> RangerReports { get; set; } = new List<RangerReport>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
