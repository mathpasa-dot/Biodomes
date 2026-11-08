namespace BioDomes.Infrastructures.Model;

public class BiomeSpeciesHistory
{
    public int Id { get; set; }

    public int BiomeId { get; set; }
    public Biome Biome { get; set; }

    public int SpeciesId { get; set; }
    public Species Species { get; set; }

    public int Delta { get; set; } // +5, -3, etc.
    public int NewCount { get; set; } // optionnel mais pratique

    public DateTime Timestamp { get; set; }
}