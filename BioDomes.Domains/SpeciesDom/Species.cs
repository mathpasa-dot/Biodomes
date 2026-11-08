namespace BioDomes.Domains.SpeciesDom;

public record Species(
    string Name,
    SpeciesType SpeciesType,
    SpeciesRegime SpeciesRegime,
    int Height,
    double Weight,
    string? FilePath = null,
    bool IsAvailable = true,
    string? OwnerId = null
)
{
}