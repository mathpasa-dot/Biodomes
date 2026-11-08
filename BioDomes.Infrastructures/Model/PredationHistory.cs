namespace BioDomes.Infrastructures.Model;

public class PredationHistory
{
    public int Id { get; set; }

    public int BiomeId { get; set; }
    public Biome Biome { get; set; }

    public int PredatorId { get; set; }
    public Species Predator { get; set; }

    public int PreyId { get; set; }
    public Species Prey { get; set; }

    public PredationAction Action { get; set; }

    public DateTime Timestamp { get; set; }
}