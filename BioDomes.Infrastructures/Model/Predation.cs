namespace BioDomes.Infrastructures.Model;

public class Predation
{
    public int PredatorId { get; set; }
    public Species Predator { get; set; }

    public int PreyId { get; set; }
    public Species Prey { get; set; }

    public int BiomeId { get; set; }
    public Biome Biome { get; set; }
}