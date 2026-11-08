using BioDomes.Domains;
using BioDomes.Domains.Dto;
using BioDomes.Domains.SpeciesDom;
using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Species = BioDomes.Domains.SpeciesDom.Species;
using SpeciesRegime = BioDomes.Domains.SpeciesDom.SpeciesRegime;
using SpeciesType = BioDomes.Domains.SpeciesDom.SpeciesType;

namespace BioDomes.Infrastructures;

public class SpeciesRepository : ISpeciesRepository
{
    private readonly BioDomesDbContext _context;

    public SpeciesRepository(BioDomesDbContext context)
    {
        _context = context;
    }

    public int Count()
    {
        return _context.Species.Count();
    }

    public IList<Species> GetAll()
    {
        return _context.Species.AsNoTracking().ToList().Select(s => ToDomain(s)).ToList();
    }

    public IList<Species> GetAvailableAndOwn(string userId)
    {
        return _context.Species.AsNoTracking()
            .Where(s => s.IsAvailable || s.UserId == userId)
            .ToList()
            .Select(s => ToDomain(s))
            .ToList();
    }
    
    public Species GetTargetSpecies(string slug)
    {
        var target = _context.Species.AsNoTracking()
            .FirstOrDefault(s => s.Slug == slug);

        return ToDomain(target ?? throw new InvalidOperationException("Espèce inconnue ou données incomplètes."));
    }
    
    public Dictionary<Species, int> GetPopulationForBiome(Biome biomeInfra)
    {
        var population = new Dictionary<Species, int>();

        foreach (var biomeSpecies in biomeInfra.BiomeSpecies)
        {
            var speciesInfra = _context.Species.AsNoTracking().FirstOrDefault(s => s.SpeciesId == biomeSpecies.SpeciesId);
            if (speciesInfra != null)
            {
                var speciesDomain = ToDomain(speciesInfra);
                population[speciesDomain] = biomeSpecies.Count;
            }
        }

        return population;
    }

    public void AddOne(Species species, string userId)
    {
        if (SpeciesExists(species))
            throw new InvalidOperationException($"L'espèce '{species.Name}' existe déjà.");

        _context.Species.Add(ToInfrastructure(species, userId));
        _context.SaveChanges();
    }

    public void UpdateOne(Species oldSpecies, Species newSpecies)
    {
        if (_context.Species
            .Any(s => s.Slug == newSpecies.Name.ToKebabCase()
                      && s.Slug != oldSpecies.Name.ToKebabCase()))
            throw new InvalidOperationException($"L'espèce '{newSpecies.Name}' existe déjà.");
        
        var speciesInfra = _context.Species
            .FirstOrDefault(s => s.Slug == oldSpecies.Name.ToKebabCase());

        if (speciesInfra is null)
            throw new InvalidOperationException($"La référence existante n'a pas pu être trouvée.");

        speciesInfra.Name = newSpecies.Name;
        speciesInfra.Slug = newSpecies.Name.ToKebabCase();
        speciesInfra.TypeId = (int)newSpecies.SpeciesType;
        speciesInfra.RegimeId = (int)newSpecies.SpeciesRegime;
        speciesInfra.Height = newSpecies.Height;
        speciesInfra.Weight = newSpecies.Weight;
        speciesInfra.ImagePath = newSpecies.FilePath;

        _context.Species.Update(speciesInfra);
        _context.SaveChanges();
    }

    public void RemoveOne(string slug)
    {
        var species = _context.Species
            .FirstOrDefault(s => s.Slug == slug);

        if (species is null)
            throw new InvalidOperationException("Espèce inconnue.");

        _context.Species.Remove(species);
        _context.SaveChanges();
    }

    public void MakeAvailable(string slug)
    {
        var species = _context.Species
            .FirstOrDefault(s => s.Slug == slug);

        if (species is null)
            throw new InvalidOperationException("Espèce inconnue.");

        species.IsAvailable = true;
        _context.SaveChanges();
    }

    internal Species ToDomain(Model.Species speciesInfra)
    {
        return new Species(speciesInfra.Name,
            (SpeciesType)speciesInfra.TypeId,
            (SpeciesRegime)speciesInfra.RegimeId,
            speciesInfra.Height,
            speciesInfra.Weight,
            speciesInfra.ImagePath,
            speciesInfra.IsAvailable,
            speciesInfra.UserId);
    }

    private Model.Species ToInfrastructure(Species speciesDomain, string userId)
    {
        return new Model.Species
        {
            Name = speciesDomain.Name,
            Slug = speciesDomain.Name.ToKebabCase(),
            TypeId = (int)speciesDomain.SpeciesType,
            RegimeId = (int)speciesDomain.SpeciesRegime,
            Height = speciesDomain.Height,
            Weight = speciesDomain.Weight,
            UserId = userId,
            IsAvailable = false,
            ImagePath = speciesDomain.FilePath
        };
    }
    
    public List<BiomeImpactDto> GetBiomesForSpecies(string slug)
    {
        var species = _context.Species
            .FirstOrDefault(s => s.Slug == slug);

        if (species == null)
            return new List<BiomeImpactDto>();
        
        var speciesId = species.SpeciesId;
        
        var predation = _context.Predations
            .Where(p => p.PredatorId == speciesId || p.PreyId == speciesId)
            .GroupBy(p => p.Biome.Name)
            .Select(g => new
            {
                BiomeName = g.Key,
                PredationCount = g.Count()
            })
            .ToList();

        var presence = _context.BiomeSpecies
            .Where(bs => bs.SpeciesId == speciesId)
            .Select(bs => new
            {
                BiomeName = bs.Biome.Name,
                PresenceCount = bs.Count
            })
            .ToList();

        var result = predation
            .Select(p => new BiomeImpactDto
            {
                BiomeName = p.BiomeName,
                PredationCount = p.PredationCount,
                PresenceCount = presence
                    .FirstOrDefault(x => x.BiomeName == p.BiomeName)?.PresenceCount ?? 0
            })
            .ToList();

        // Ajouter les biomes où il y a présence mais pas de prédation
        var missing = presence
            .Where(p => result.All(r => r.BiomeName != p.BiomeName))
            .Select(p => new BiomeImpactDto
            {
                BiomeName = p.BiomeName,
                PredationCount = 0,
                PresenceCount = p.PresenceCount
            });

        result.AddRange(missing);

        return result;
    }
    
    private bool SpeciesExists(Species species)
    {
        return _context.Species.Any(s => s.Slug.Equals(species.Name.ToKebabCase()));;
    }
}