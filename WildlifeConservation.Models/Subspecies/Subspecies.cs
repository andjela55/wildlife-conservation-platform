namespace WildlifeConservation.Models.Subspecies;

public class Subspecies
{
    public int Id { get; set; }
    public int SpeciesId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Models.Species.Species? Species { get; set; }
    public ICollection<Animal> Animals { get; set; } = new List<Animal>();
}
