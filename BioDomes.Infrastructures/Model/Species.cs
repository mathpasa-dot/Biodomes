namespace BioDomes.Infrastructures.Model;

public class Species
{
    public int SpeciesId { get; set; }
    
    public string Slug { get; set; }
    public string Name { get; set; }

    public string? ImagePath { get; set; }

    public int Height { get; set; }
    public double Weight { get; set; }
    public int RegimeId { get; set; }
    public SpeciesRegime Regime { get; set; }
    public int TypeId { get; set; }
    public SpeciesType Type { get; set; }

    public bool IsAvailable { get; set; } = false;

    public string UserId { get; set; }
    public User User { get; set; }

    public List<BiomeSpecies> BiomeSpecies { get; set; } = new();

    public List<Predation> AsPredator { get; set; } = new(); // relations où cette espèce est prédateur
    public List<Predation> AsPrey { get; set; } = new(); // relations où cette espèce est proie
    
    public List<BiomeSpeciesHistory> BiomeSpeciesHistory { get; set; } = new();
}