namespace BioDomes.Infrastructures.Model;

public class BiomeEquipment
{
    public int BiomeId { get; set; }
    public Biome Biome { get; set; }

    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; }

    public int Count { get; set; }
}