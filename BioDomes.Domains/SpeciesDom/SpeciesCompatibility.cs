namespace BioDomes.Domains.SpeciesDom;

public static class SpeciesCompatibility
{
    private static readonly HashSet<SpeciesType> PhotosyntheticTypes =
    [
        SpeciesType.Plant, SpeciesType.Moss, SpeciesType.Algae, SpeciesType.Lichen
    ];

    public static bool IsRegimeCompatible(SpeciesType type, SpeciesRegime regime)
    {
        if (PhotosyntheticTypes.Contains(type))
            return regime == SpeciesRegime.PhotoSynthesis;

        if (type == SpeciesType.Mushroom)
            return regime is SpeciesRegime.Detritivores or SpeciesRegime.Heterotrophie;

        // Animaux : tout sauf la photosynthèse
        return regime != SpeciesRegime.PhotoSynthesis;
    }
}