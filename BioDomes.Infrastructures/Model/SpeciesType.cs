namespace BioDomes.Infrastructures.Model;

public class SpeciesType
{
    public int TypeId { get; set; }
    public string Label { get; set; }

    public List<Species> Species { get; set; } = new();
}