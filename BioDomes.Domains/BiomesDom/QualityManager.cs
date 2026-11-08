using BioDomes.Domains.Dto;
using BioDomes.Domains.SpeciesDom;

namespace BioDomes.Domains.BiomesDom;

public class QualityManager
{
    public ScoreDto ComputeScore(Biome biome)
    {
        var balanceScore   = ComputeBalanceScore(biome);
        var diversityScore = ComputeDiversityScore(biome);
        var total          = Math.Clamp((int)Math.Round(balanceScore + diversityScore), 0, 100);

        return new ScoreDto(
            Total:          total,
            BalanceScore:   (int)Math.Round(balanceScore   / 70.0 * 100),
            DiversityScore: (int)Math.Round(diversityScore / 30.0 * 100)
        );
    }

    private double ComputeDiversityScore(Biome biome)
    {
        var speciesCount = biome.Population.Keys.Count;
        var regimeCount = biome.Population.Keys
            .Select(s => s.SpeciesRegime)
            .Distinct()
            .Count();

        // Nombre d'espèces : plafonné à 20 espèces = score max
        var speciesScore = Math.Min(speciesCount / 20.0, 1.0) * 20;

        // Diversité des régimes : plus il y a de régimes différents, mieux c'est
        var regimeScore = Math.Min(regimeCount / 5.0, 1.0) * 10;

        return speciesScore + regimeScore;
    }
    
    public IList<HungrySpeciesWarning> IsBiomeInBalance(Biome biome)
    {
        var comments = new List<HungrySpeciesWarning>();
        
        /*var predators = biome.Predations
            .Select(p => p.Predator)
            .ToHashSet();

        var preys = biome.Predations
            .Select(p => p.Prey)
            .ToHashSet();
        
        var isolatedAnimals = biome.Population.Keys
            .Where(s => !FoodChainService.IsPlant(s) && !predators.Contains(s) && preys.Contains(s))
            .ToList();

        foreach (var animal in isolatedAnimals)
        {
            var mass = animal.Weight * biome.Population[animal];
            comments.Add(new HungrySpeciesWarning(animal.Name, $"L'animal \"{animal.Name}\" hétérotrophe n'a pas de proies. " +
                $"Masse nutritionnelle nécessaire : {Math.Round(mass)} kg.", true));
        }*/

        var required = ComputeRequiredMasses(biome);

        foreach (var (species, requiredMass) in required)
        {
            if (!biome.Population.ContainsKey(species)) continue;

            var actualMass = species.Weight * biome.Population[species];
            const double tolerance = 0.1;

            if (actualMass < requiredMass * (1 - tolerance))
            {
                comments.Add( new HungrySpeciesWarning(species.Name, $"\"{species.Name}\" a une masse insuffisante. " +
                     $"Requis : {Math.Round(requiredMass)} kg. " +
                     $"Réel : {Math.Round(actualMass)} kg. " +
                     $"Déficit : {Math.Round(requiredMass - actualMass)} kg.", false));
            }
        }

        return comments;
    }

    private double ComputeBalanceScore(Biome biome)
    {
        var predators = biome.Predations
            .Select(p => p.Predator)
            .ToHashSet();

        var preys = biome.Predations
            .Select(p => p.Prey)
            .ToHashSet();

        var isolatedAnimals = biome.Population.Keys
            .Where(s => !FoodChainService.IsPlant(s) && !predators.Contains(s) && preys.Contains(s))
            .ToList();

        var required = ComputeRequiredMasses(biome);

        // Participants = toutes les espèces avec un requis
        var totalParticipants = required.Count;
        if (totalParticipants == 0) return 0;

        double totalScore = 0;

        foreach (var (species, requiredMass) in required)
        {
            if (!biome.Population.ContainsKey(species)) continue;
            if (requiredMass == 0) continue;

            var actualMass = species.Weight * biome.Population[species];
            var ratio = actualMass / requiredMass;

            var speciesScore = ratio switch
            {
                >= 0.9 and <= 1.1 => 100,
                >= 0.7 and <= 1.5 => 70,
                >= 0.5 and <= 2.0 => 40,
                _                 => 10
            };

            totalScore += speciesScore;
        }

        // Animaux isolés baisse la note
        var isolatedPenalty = isolatedAnimals.Count * 5;
        return Math.Max(0, totalScore / totalParticipants / 100.0 * 70 - isolatedPenalty);
    }
    
    private Dictionary<Species, double> ComputeRequiredMasses(Biome biome)
    {
        var required = new Dictionary<Species, double>();

        // Initialisation : apex prédateurs = leur masse réelle
        var preySpecies = biome.Predations.Select(p => p.Prey).ToHashSet();
        
        var apexPredators = biome.Predations
            .Select(p => p.Predator)
            .Where(s => !preySpecies.Contains(s))
            .Distinct();

        foreach (var apex in apexPredators)
            required[apex] = apex.Weight * biome.Population[apex];

        // Propagation top-down niveau par niveau (BFS)
        var queue = new Queue<Species>(required.Keys);

        while (queue.Count > 0)
        {
            var predator = queue.Dequeue();
            var predatorRequired = required[predator];

            var preys = biome.Predations
                .Where(p => p.Predator == predator)
                .Select(p => p.Prey);

            foreach (var prey in preys)
            {
                // Chaque proie doit satisfaire la somme de tous ses prédateurs
                var contribution = predatorRequired; // ce prédateur exige ce poids de cette proie

                if (!required.ContainsKey(prey))
                    required[prey] = 0;

                var oldRequired = required[prey];
                required[prey] += contribution; // accumulation si proie partagée

                // On propage vers le bas seulement si le requis a augmenté
                if (required[prey] > oldRequired)
                    queue.Enqueue(prey);
            }
        }

        return required;
    }

    public IList<string> GenerateSuggestions(Biome biome, IList<HungrySpeciesWarning> warnings)
    {
        List<string> suggestions = new List<string>();
        
        /*var suggestionsBySpecies = biome.Predations
            .GroupBy(p => p.Predator)
            .ToDictionary(
                g => g.Key,
                g => g.Select(p => p.Prey.Name).Distinct().ToList()
            );

        foreach (var warning in warnings.Where(h => h.OnlyPrey == true))
        {
            suggestions.Add(
                $"{warning.Name} : ajouter une nouvelle espèce comme proie."
            );
        }*/

        foreach (var warning in warnings.Where(h => h.OnlyPrey == false))
        {
            /*var key = suggestionsBySpecies.Keys
                .FirstOrDefault(s => s.Name.ToKebabCase() == warning.Name.ToKebabCase());

            if (key == null)
            {*/
                suggestions.Add(
                    $"{warning.Name} : ajouter des individus."
                );
                /*continue;
            }

            var preyList = suggestionsBySpecies[key];

            if (preyList.Count == 0) continue;

            suggestions.Add(
                $"{warning.Name} : ajouter {string.Join(", ", preyList)}."
            );*/
        }
        
        return suggestions;
    }
}