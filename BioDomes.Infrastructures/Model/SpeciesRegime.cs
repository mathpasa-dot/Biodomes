namespace BioDomes.Infrastructures.Model;

public class SpeciesRegime
{
    public int RegimeId { get; set; }
    public string Label { get; set; }

    public List<Species> Species { get; set; } = new();
}