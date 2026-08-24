using System.ComponentModel.DataAnnotations;

namespace WildlifeConservation.DTOs;

public class CollarAssignmentQuery : PaginationQuery
{
    [Range(1, int.MaxValue)]
    public int? AnimalId { get; set; }
    public DateTime? AssignedFrom { get; set; }
    public DateTime? AssignedTo { get; set; }
    public bool? ActiveOnly { get; set; }
}
