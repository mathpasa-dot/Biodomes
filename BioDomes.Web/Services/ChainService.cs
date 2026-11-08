using System.Text.Json;
using BioDomes.Domains;
using BioDomes.Domains.BiomesDom;
using BioDomes.Domains.Dto;
using BioDomes.Domains.SpeciesDom;

namespace BioDomes.Web.Services;

public class ChainService
{
    public string JsonChain(Biome biome)
    {
        var speciesLevels = ComputeLevels(biome);

        var speciesData = biome.Population.Select(kvp => new
        {
            name = kvp.Key.Name.ToKebabCase(),
            type = kvp.Key.SpeciesType.ToDisplayString(),
            regime = kvp.Key.SpeciesRegime.ToDisplayString(),
            quantity = kvp.Value,
            weight = kvp.Key.Weight,
            level = speciesLevels.TryGetValue(kvp.Key.Name, out var l) ? l : 0
        }).ToList();

        var predationData = biome.Predations.Select(p => new
        {
            predator = p.Predator.Name.ToKebabCase(),
            prey = p.Prey.Name.ToKebabCase()
        }).ToList();

        return JsonSerializer.Serialize(new
        {
            species = speciesData,
            predations = predationData
        });
    }
    
    public object JsonDashboard(Biome biome, ScoreDto score,DateTime minDate, DateTime maxDate, 
        IList<TimeEventsDto> lineComments, IList<HungrySpeciesWarning> balanceComments)
    {
        var speciesLevels = ComputeLevels(biome);

        var speciesData = biome.Population.Select(kvp => new
        {
            name = kvp.Key.Name,
            type = kvp.Key.SpeciesType.ToDisplayString(),
            regime = kvp.Key.SpeciesRegime.ToDisplayString(),
            quantity = kvp.Value,
            weight = kvp.Key.Weight,
            level = speciesLevels.TryGetValue(kvp.Key.Name, out var l) ? l : 0
        }).ToList();

        var predationData = biome.Predations.Select(p => new
        {
            predator = p.Predator.Name,
            prey = p.Prey.Name
        }).ToList();
        
        return new
        {
            biomeName = biome.Name,
            temperature = biome.Temperature,
            moisture = biome.Moisture,
            score,
            species = speciesData,
            predations = predationData,

            timeline = new
            {
                minDate,
                maxDate
            },

            timelineEvents = lineComments,
            balanceComments = balanceComments.Select(c => c.Warning).ToList()
        };
    }
    
    public List<string> GetConflictingSpecies(List<string> predators, List<string> prey)
    {
        predators ??= new List<string>();
        prey ??= new List<string>();

        var predatorSet = predators
            .Select(p => p.ToKebabCase())
            .ToHashSet();

        var preySet = prey
            .Select(p => p.ToKebabCase())
            .ToHashSet();

        return predatorSet
            .Intersect(preySet)
            .ToList();
    }

    private Dictionary<string, int> ComputeLevels(Biome biome)
    {
        var level = biome.Population.Keys.ToDictionary(s => s.Name, _ => 0);
        for (var i = 0; i < biome.Population.Count; i++)
            foreach (var p in biome.Predations)
                if (level.ContainsKey(p.Predator.Name) && level.ContainsKey(p.Prey.Name))
                    level[p.Prey.Name] = Math.Max(level[p.Prey.Name], level[p.Predator.Name] + 1);
        return level;
    }
}