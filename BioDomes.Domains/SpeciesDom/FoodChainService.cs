namespace BioDomes.Domains.SpeciesDom;

public class FoodChainService
{
    private static readonly Dictionary<SpeciesType, int> _trophicLevels = new()
    {
        // Level 1 — Producers
        [SpeciesType.Plant] = 1,
        [SpeciesType.Moss] = 1,
        [SpeciesType.Algae] = 1,
        [SpeciesType.Lichen] = 1,
        [SpeciesType.Porifera] = 1,

        // Level 2 — Primary consumers
        [SpeciesType.Insect] = 2,
        [SpeciesType.Mollusc] = 2,
        [SpeciesType.Crustacean] = 2,
        [SpeciesType.Myriapods] = 2,
        [SpeciesType.Echinoderms] = 2,
        [SpeciesType.Anelids] = 2,
        [SpeciesType.Cnidarians] = 2,
        [SpeciesType.Flatworms] = 2,
        [SpeciesType.Mushroom] = 2,

        // Level 3 — Secondary consumers
        [SpeciesType.Arachnid] = 3,
        [SpeciesType.Fish] = 3,
        [SpeciesType.Amphibian] = 3,

        // Level 4 — Apex / tertiary
        [SpeciesType.Reptile] = 4,
        [SpeciesType.Bird] = 4,
        [SpeciesType.Mammal] = 4
    };
    
    private static readonly HashSet<SpeciesType> _detritusActors = new()
    {
        SpeciesType.Mushroom,
        SpeciesType.Anelids,
        SpeciesType.Flatworms
    };
    
    private static readonly HashSet<SpeciesType> _excrementActors = new()
    {
        // espèces typiquement coprophages ou liées aux excréments
        SpeciesType.Insect,      // ex: mouches, scarabées
        SpeciesType.Anelids      // certains vers
    };

    private static readonly HashSet<SpeciesType> _mudActors = new()
    {
        // organismes vivant dans/sur la vase
        SpeciesType.Mollusc,
        SpeciesType.Anelids,
        SpeciesType.Crustacean
    };

    private static readonly Dictionary<SpeciesRegime, Func<Species, bool>> _feedingRules
        = new()
    {
        [SpeciesRegime.Herbivore] = (prey) =>
            IsPlant(prey),

        [SpeciesRegime.Carnivore] = (prey) =>
            IsAnimal(prey),

        [SpeciesRegime.Omnivore] = (prey) =>
            IsPlant(prey) || IsAnimal(prey),

        [SpeciesRegime.Detritivores] = (prey) =>
            IsDeadOrganicMatter(prey),

        [SpeciesRegime.Coprophages] = (prey) =>
            IsExcrement(prey),

        [SpeciesRegime.Limivores] = (prey) =>
            IsMudOrganicMatter(prey),

        [SpeciesRegime.PhotoSynthesis] = (_) => false,

        [SpeciesRegime.Heterotrophie] = (prey) => true // fallback large
    };
    
    public bool FirstCanEatSecond(Species predator, Species prey)
    {
        if (predator.Name.ToKebabCase() == prey.Name.ToKebabCase())
            return false;

        if (!_feedingRules.TryGetValue(predator.SpeciesRegime, out var rule))
            return false;

        if (!rule(prey))
            return false;

        // Ajustement trophique (plus souple)
        var levelDiff = GetTrophicLevel(predator) - GetTrophicLevel(prey);

        if (levelDiff < -1)
            return false;

        return true;
    }
    
    public static bool IsPlant(Species s) =>
        s.SpeciesType is SpeciesType.Plant
            or SpeciesType.Moss
            or SpeciesType.Algae
            or SpeciesType.Lichen;

    private static bool IsAnimal(Species s) =>
        s.SpeciesType is >= SpeciesType.Mammal and <= SpeciesType.Porifera;

    private static bool IsDeadOrganicMatter(Species s)
    {
        return _detritusActors.Contains(s.SpeciesType);
    }
    
    private static bool IsExcrement(Species s)
    {
        return _excrementActors.Contains(s.SpeciesType);
    }

    private static bool IsMudOrganicMatter(Species s)
    {
        return _mudActors.Contains(s.SpeciesType);
    }

    private int GetTrophicLevel(Species s)
    {
        _trophicLevels.TryGetValue(s.SpeciesType, out var level);
        return level;
    }
}