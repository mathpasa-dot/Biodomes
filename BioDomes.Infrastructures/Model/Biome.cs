namespace BioDomes.Infrastructures.Model;

public class Biome
{
    public int BiomeId { get; set; }
    
    public string Slug { get; set; }
    public string Name { get; set; }
    public decimal Temperature { get; set; }
    public decimal Humidity { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }

    public List<BiomeSpecies> BiomeSpecies { get; set; } = new();

    public List<BiomeEquipment> BiomeEquipments { get; set; } = new();

    public List<Predation> Predations { get; set; } = new();
    
    public List<BiomeSpeciesHistory> BiomeSpeciesHistory { get; set; } = new();
}