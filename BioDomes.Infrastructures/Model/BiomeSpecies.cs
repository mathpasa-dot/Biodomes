namespace BioDomes.Infrastructures.Model;

public class BiomeSpecies
{
    public int BiomeId { get; set; }
    public Biome Biome { get; set; }

    public int SpeciesId { get; set; }
    public Species Species { get; set; }

    public int Count { get; set; }
}