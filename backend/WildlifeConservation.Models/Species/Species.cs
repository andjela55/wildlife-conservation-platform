namespace WildlifeConservation.Models.Species;

public class Species
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<Models.Subspecies.Subspecies> Subspecies { get; set; } = new List<Models.Subspecies.Subspecies>();
}
