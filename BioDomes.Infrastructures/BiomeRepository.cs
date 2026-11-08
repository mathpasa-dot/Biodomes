using BioDomes.Domains;
using BioDomes.Domains.BiomesDom;
using BioDomes.Domains.Dto;
using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Biome = BioDomes.Domains.BiomesDom.Biome;
using Predation = BioDomes.Infrastructures.Model.Predation;

namespace BioDomes.Infrastructures;

public class BiomeRepository : IBiomeRepository
{
    private readonly BioDomesDbContext _context;

    public BiomeRepository(BioDomesDbContext context)
    {
        _context = context;
    }
    
    public int Count()
    {
        return _context.Biomes.Count();
    }

    public IList<Biome> GetAll()
    {
        var speRepo = new SpeciesRepository(_context);
        var equipRepo = new EquipmentRepository(_context);
        
        return _context.Biomes.AsNoTracking().Include(b => b.BiomeSpecies)
            .Include(b => b.BiomeEquipments)
            .Include(b => b.Predations)
            .ThenInclude(p => p.Predator)
            .Include(b => b.Predations)
            .ThenInclude(p => p.Prey).ToList().Select(b => ToDomain(b, speRepo, equipRepo)).ToList();
    }

    public IList<Biome> GetAllForUser(string userId)
    {
        var speRepo = new SpeciesRepository(_context);
        var equipRepo = new EquipmentRepository(_context);
        
        return _context.Biomes.AsNoTracking()
            .Include(b => b.BiomeSpecies)
            .Include(b => b.BiomeEquipments)
            .Include(b => b.Predations)
            .ThenInclude(p => p.Predator)
            .Include(b => b.Predations)
            .ThenInclude(p => p.Prey)
            .Where(b => b.UserId == userId)
            .ToList()
            .Select(b => ToDomain(b, speRepo, equipRepo))
            .ToList();
    }
    
    public Biome GetTargetBiome(string biomeSlug)
    {
        var speRepo = new SpeciesRepository(_context);
        var equipRepo = new EquipmentRepository(_context);
        
        var biomeInfra = _context.Biomes.AsNoTracking().AsSplitQuery()
            .Include(b => b.BiomeSpecies)
            .Include(b => b.BiomeEquipments)
            .Include(b => b.Predations)
            .ThenInclude(p => p.Predator)
            .Include(b => b.Predations)
            .ThenInclude(p => p.Prey)
            .FirstOrDefault(b => b.Slug == biomeSlug);
        
        return ToDomain(biomeInfra ?? throw new InvalidOperationException("Biome inconnu ou données incomplètes."), speRepo,
            equipRepo);
    }

    public IList<BiomeCardModel> GetBestBiome(int count)
    {
        QualityManager qm = new QualityManager();
        
        return GetAll().Where(b => qm.IsBiomeInBalance(b).Count == 0)
            .Select((b, index) => new
            {
                Biome = b,
                Score = qm.ComputeScore(b).Total,
                Index = index // pour gérer les égalités
            })
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Index)
            .Take(count)
            .Select(x => new BiomeCardModel(x.Biome.Name, x.Biome.Temperature, x.Biome.Moisture, 
                x.Score, x.Biome.Population.Count))
            .ToList();
    }

    public IList<Biome> GetByFilter(double? tempMin, double? tempMax, double? moistureMin, double? moistureMax, string? species, string? equipment)
    {
        var speRepo = new SpeciesRepository(_context);
        var equipRepo = new EquipmentRepository(_context);

        return _context.Biomes.AsNoTracking()
            .Include(b => b.BiomeSpecies).ThenInclude(bs => bs.Species)
            .Include(b => b.BiomeEquipments).ThenInclude(be => be.Equipment)
            .Include(b => b.Predations).ThenInclude(p => p.Predator)
            .Include(b => b.Predations).ThenInclude(p => p.Prey)
            .ToList()
            .Where(b => tempMin == null || b.Temperature >= (decimal)tempMin)
            .Where(b => tempMax == null || b.Temperature <= (decimal)tempMax)
            .Where(b => moistureMin == null || b.Humidity >= (decimal)moistureMin)
            .Where(b => moistureMax == null || b.Humidity <= (decimal)moistureMax)
            .Where(b => string.IsNullOrWhiteSpace(species) ||
                        b.BiomeSpecies.Any(bs => bs.Species.Name.Contains(species,
                            StringComparison.OrdinalIgnoreCase)))
            .Where(b => string.IsNullOrWhiteSpace(equipment) ||
                        b.BiomeEquipments.Any(be => be.Equipment.Name.Contains(equipment,
                            StringComparison.OrdinalIgnoreCase)))
            .Select(b => ToDomain(b, speRepo, equipRepo))
            .Where(b => new QualityManager().IsBiomeInBalance(b).Count == 0).ToList();
    }

    public void AddOne(Biome biome, string userId)
    {
        if (BiomeExists(biome))
            throw new InvalidOperationException($"Le biome '{biome.Name}' existe déjà.");

        _context.Biomes.Add(ToInfrastructure(biome, userId));
        _context.SaveChanges();
    }

    public void UpdateOne(Biome oldBiome, Biome newBiome)
    { 
        if (_context.Biomes.Any(b => b.Slug == newBiome.Name.ToKebabCase()
                                     && b.Slug != oldBiome.Name.ToKebabCase()))
            throw new InvalidOperationException($"Le biome '{newBiome.Name}' existe déjà.");

        var biomeInfra = _context.Biomes.FirstOrDefault(b => b.Slug == oldBiome.Name.ToKebabCase());

        if (biomeInfra is null) 
            throw new InvalidOperationException($"La référence existante n'a pas pu être trouvée.");

        biomeInfra.Name = newBiome.Name;
        biomeInfra.Slug = newBiome.Name.ToKebabCase();
        biomeInfra.Temperature = Convert.ToDecimal(newBiome.Temperature);
        biomeInfra.Humidity = Convert.ToDecimal(newBiome.Moisture);

        _context.SaveChanges();
    }

    public void RemoveOne(string slug)
    {
        var biome = _context.Biomes
            .FirstOrDefault(b => b.Slug == slug);

        if (biome is null)
            throw new InvalidOperationException("Biome inconnu.");

        _context.Biomes.Remove(biome);
        _context.SaveChanges();
    }

    // Si l'espèce existe déjà, update du nombre d'individus
    public void AddSpeciesToBiome(string biomeSlug, string speciesSlug, int count)
    {
        var biome = _context.Biomes
            .Include(b => b.BiomeSpecies)
            .FirstOrDefault(b => b.Slug == biomeSlug);

        if (biome is null)
            throw new InvalidOperationException("Biome inconnu.");

        var species = _context.Species
            .FirstOrDefault(s => s.Slug == speciesSlug);

        if (species is null)
            throw new InvalidOperationException("Espèce inconnue.");

        var existing = biome.BiomeSpecies
            .FirstOrDefault(bs => bs.SpeciesId == species.SpeciesId);
        
        int oldCount = 0;
        int delta = 0;

        if (existing is not null)
        {
            oldCount = existing.Count;
            existing.Count = count;
        }
        else
        {
            biome.BiomeSpecies.Add(new BiomeSpecies
            {
                BiomeId = biome.BiomeId,
                SpeciesId = species.SpeciesId,
                Count = count
            });
        }
        
        delta = count - oldCount;

        // 👉 Ajouter une entrée dans l'historique UNIQUEMENT si changement
        if (delta != 0)
        {
            _context.BiomeHistory.Add(new BiomeSpeciesHistory
            {
                BiomeId = biome.BiomeId,
                SpeciesId = species.SpeciesId,
                Delta = delta,
                NewCount = count,
                Timestamp = DateTime.UtcNow
            });
        }

        _context.SaveChanges();
    }

    // Si l'équipement existe déjà, update du nombre d'individus
    public void AddEquipToBiome(string biomeSlug, string equipSlug, int count)
    {
        var biome = _context.Biomes
            .Include(b => b.BiomeEquipments)
            .FirstOrDefault(b => b.Slug == biomeSlug);

        if (biome is null)
            throw new InvalidOperationException("Biome inconnu.");

        var equipment = _context.Equipments
            .FirstOrDefault(s => s.Slug == equipSlug);

        if (equipment is null)
            throw new InvalidOperationException("Équipement inconnu.");

        var existing = biome.BiomeEquipments
            .FirstOrDefault(bs => bs.EquipmentId == equipment.EquipmentId);

        if (existing is not null)
            existing.Count = count;
        else
            biome.BiomeEquipments.Add(new BiomeEquipment
            {
                BiomeId = biome.BiomeId,

                EquipmentId = equipment.EquipmentId,
                Count = count
            });

        _context.SaveChanges();
    }

    public void RemoveSpeciesFromBiome(string biomeSlug, string speciesSlug)
    {
        var biomeId = _context.Biomes
            .Where(b => b.Slug == biomeSlug)
            .Select(b => (int?)b.BiomeId)
            .FirstOrDefault();

        if (biomeId is null)
            throw new InvalidOperationException("Biome inconnu.");

        var speciesId = _context.Species
            .Where(s => s.Slug == speciesSlug)
            .Select(s => (int?)s.SpeciesId)
            .FirstOrDefault();

        if (speciesId is null)
            throw new InvalidOperationException("Espèce inconnue.");
        
        var relation = _context.BiomeSpecies
            .FirstOrDefault(bs => bs.BiomeId == biomeId && bs.SpeciesId == speciesId);

        if (relation is null)
            throw new InvalidOperationException("Relation existante inconnue.");
        
        int oldCount = relation.Count;
        int newCount = 0;
        int delta = newCount - oldCount; // donc négatif
        
        if (delta != 0)
        {
            _context.BiomeHistory.Add(new BiomeSpeciesHistory
            {
                BiomeId = biomeId.Value,
                SpeciesId = speciesId.Value,
                Delta = delta,
                NewCount = newCount,
                Timestamp = DateTime.UtcNow
            });
        }
        
        var predationsToRemove = _context.Predations
            .Where(p =>
                p.BiomeId == biomeId &&
                (p.PredatorId == speciesId || p.PreyId == speciesId)).ToList();
        
        foreach (var p in predationsToRemove)
        {
            _context.PredationHistory.Add(new PredationHistory
            {
                BiomeId = p.BiomeId,
                PredatorId = p.PredatorId,
                PreyId = p.PreyId,
                Action = PredationAction.Removed,
                Timestamp = DateTime.UtcNow
            });
        }

        _context.BiomeSpecies.Remove(relation);
        _context.Predations.RemoveRange(predationsToRemove);

        _context.SaveChanges();
    }

    public void RemoveEquipFromBiome(string biomeSlug, string equipSlug)
    {
        var biomeId = _context.Biomes
            .Where(b => b.Slug == biomeSlug)
            .Select(b => (int?)b.BiomeId)
            .FirstOrDefault();

        if (biomeId is null)
            throw new InvalidOperationException("Biome inconnu.");
        
        var equipmentId = _context.Equipments
            .Where(e => e.Slug == equipSlug)
            .Select(e => (int?)e.EquipmentId)
            .FirstOrDefault();

        if (equipmentId is null)
            throw new InvalidOperationException("Équipement inconnu.");
        
        var relationsToRemove = _context.BiomeEquipments
            .Where(be => be.BiomeId == biomeId && be.EquipmentId == equipmentId);

        _context.BiomeEquipments.RemoveRange(relationsToRemove);

        _context.SaveChanges();
    }

    public void AddPredation(string biomeSlug, string predatorSlug, string preySlug)
    {
        var biomeId = _context.Biomes
                          .Where(b => b.Slug == biomeSlug)
                          .Select(b => (int?)b.BiomeId)
                          .FirstOrDefault()
                      ?? throw new InvalidOperationException("Biome inconnu.");

        var predatorId = _context.Species
                             .Where(s => s.Slug == predatorSlug)
                             .Select(s => (int?)s.SpeciesId)
                             .FirstOrDefault()
                         ?? throw new InvalidOperationException("Prédateur inconnu.");

        var preyId = _context.Species
                         .Where(s => s.Slug == preySlug)
                         .Select(s => (int?)s.SpeciesId)
                         .FirstOrDefault()
                     ?? throw new InvalidOperationException("Proie inconnue.");

        var alreadyExists = _context.Predations.Any(p =>
            p.BiomeId == biomeId &&
            p.PredatorId == predatorId &&
            p.PreyId == preyId);

        if (!alreadyExists)
        {
            _context.Predations.Add(new Predation
            {
                BiomeId = biomeId,
                PredatorId = predatorId,
                PreyId = preyId
            });
            
            _context.PredationHistory.Add(new PredationHistory
            {
                BiomeId = biomeId,
                PredatorId = predatorId,
                PreyId = preyId,
                Action = PredationAction.Added,
                Timestamp = DateTime.UtcNow
            });

            _context.SaveChanges();
        }
    }
    
    public void UpdatePredationsForSpecies(string biomeSlug, string speciesSlug, 
    List<string> newPredatorSlugs, List<string> newPreySlugs)
    {
        var biomeId = _context.Biomes
            .Where(b => b.Slug == biomeSlug)
            .Select(b => (int?)b.BiomeId)
            .FirstOrDefault()
            ?? throw new InvalidOperationException("Biome inconnu.");

        var speciesId = _context.Species
            .Where(s => s.Slug == speciesSlug)
            .Select(s => (int?)s.SpeciesId)
            .FirstOrDefault()
            ?? throw new InvalidOperationException("Espèce inconnue.");

        // IDs des nouvelles listes
        var newPredatorIds = _context.Species
            .Where(s => newPredatorSlugs.Contains(s.Slug))
            .Select(s => s.SpeciesId).ToHashSet();

        var newPreyIds = _context.Species
            .Where(s => newPreySlugs.Contains(s.Slug))
            .Select(s => s.SpeciesId).ToHashSet();

        // Relations actuelles
        var existing = _context.Predations
            .Where(p => p.BiomeId == biomeId &&
                        (p.PredatorId == speciesId || p.PreyId == speciesId))
            .ToList();

        var existingPredatorIds = existing
            .Where(p => p.PreyId == speciesId)
            .Select(p => p.PredatorId).ToHashSet();

        var existingPreyIds = existing
            .Where(p => p.PredatorId == speciesId)
            .Select(p => p.PreyId).ToHashSet();

        // Diff
        var predatorsToRemove = existingPredatorIds.Except(newPredatorIds).ToHashSet();
        var predatorsToAdd    = newPredatorIds.Except(existingPredatorIds).ToHashSet();
        var preyToRemove      = existingPreyIds.Except(newPreyIds).ToHashSet();
        var preyToAdd         = newPreyIds.Except(existingPreyIds).ToHashSet();

        // Suppressions
        var toDelete = existing.Where(p =>
            (p.PreyId == speciesId && predatorsToRemove.Contains(p.PredatorId)) ||
            (p.PredatorId == speciesId && preyToRemove.Contains(p.PreyId))).ToList();

        foreach (var p in toDelete)
        {
            _context.PredationHistory.Add(new PredationHistory
            {
                BiomeId = biomeId,
                PredatorId = p.PredatorId,
                PreyId = p.PreyId,
                Action = PredationAction.Removed,
                Timestamp = DateTime.UtcNow
            });
        }
        _context.Predations.RemoveRange(toDelete);

        // Ajouts
        foreach (var predatorId in predatorsToAdd)
        {
            _context.Predations.Add(new Predation
            {
                BiomeId = biomeId,
                PredatorId = predatorId,
                PreyId = speciesId
            });
            _context.PredationHistory.Add(new PredationHistory
            {
                BiomeId = biomeId,
                PredatorId = predatorId,
                PreyId = speciesId,
                Action = PredationAction.Added,
                Timestamp = DateTime.UtcNow
            });
        }

        foreach (var preyId in preyToAdd)
        {
            _context.Predations.Add(new Predation
            {
                BiomeId = biomeId,
                PredatorId = speciesId,
                PreyId = preyId
            });
            _context.PredationHistory.Add(new PredationHistory
            {
                BiomeId = biomeId,
                PredatorId = speciesId,
                PreyId = preyId,
                Action = PredationAction.Added,
                Timestamp = DateTime.UtcNow
            });
        }

        _context.SaveChanges();
    }
    
    public Biome GetBiomeAtDate(string biomeSlug, DateTime targetDate)
    {
        // 1. Charger le biome courant
        var biome = _context.Biomes
            .Include(b => b.BiomeSpecies)
                .ThenInclude(bs => bs.Species)
            .Include(b => b.Predations)
            .FirstOrDefault(b => b.Slug == biomeSlug)
            ?? throw new InvalidOperationException("Biome inconnu.");

        // 2. Snapshot de travail (espèces)
        var speciesCounts = biome.BiomeSpecies
            .ToDictionary(
                bs => bs.SpeciesId,
                bs => bs.Count
            );

        // 3. Snapshot des prédations
        var predations = biome.Predations
            .Select(p => new Predation
            {
                BiomeId = p.BiomeId,
                PredatorId = p.PredatorId,
                PreyId = p.PreyId
            })
            .ToList();

        // =========================================================
        // 4. Charger historique APRÈS la date (IMPORTANT)
        // =========================================================

        var speciesHistoryAfter = _context.BiomeHistory
            .Where(h => h.BiomeId == biome.BiomeId && h.Timestamp > targetDate)
            .OrderByDescending(h => h.Timestamp)
            .ToList();

        var predationHistoryAfter = _context.PredationHistory
            .Where(h => h.BiomeId == biome.BiomeId && h.Timestamp > targetDate)
            .OrderByDescending(h => h.Timestamp)
            .ToList();

        // =========================================================
        // 5. Rollback espèces
        // =========================================================

        foreach (var h in speciesHistoryAfter)
        {
            if (speciesCounts.ContainsKey(h.SpeciesId))
                speciesCounts[h.SpeciesId] -= h.Delta;
            else // ?
                speciesCounts[h.SpeciesId] = -h.Delta;

            if (speciesCounts[h.SpeciesId] <= 0)
                speciesCounts.Remove(h.SpeciesId);
        }

        // =========================================================
        // 6. Rollback prédations
        // =========================================================

        foreach (var h in predationHistoryAfter)
        {
            if (h.Action == PredationAction.Added)
            {
                // on supprime la relation (rollback)
                predations.RemoveAll(p =>
                    p.PredatorId == h.PredatorId &&
                    p.PreyId == h.PreyId);
            }
            else if (h.Action == PredationAction.Removed)
            {
                // on ré-ajoute la relation
                predations.Add(new Predation
                {
                    BiomeId = biome.BiomeId,
                    PredatorId = h.PredatorId,
                    PreyId = h.PreyId
                });
            }
        }

        // =========================================================
        // 7. Reconstruction finale du biome
        // =========================================================

        SpeciesRepository speRepo = new SpeciesRepository(_context);

        var reconstructed = new Biome(
            biome.Name,
            Convert.ToDouble(biome.Temperature),
            Convert.ToDouble(biome.Humidity),
            speciesCounts
                .Select(kvp =>
                {
                    var species = speRepo
                        .ToDomain(
                            _context.Species.First(s => s.SpeciesId == kvp.Key)
                        );

                    return new KeyValuePair<Domains.SpeciesDom.Species, int>(species, kvp.Value);
                })
                .ToDictionary(x => x.Key, x => x.Value),
            new Dictionary<Domains.EquipmentsDom.Equipment, int>(),
            predations
                .Select(p => new Domains.Predation(
                    speRepo.ToDomain(
                        _context.Species.First(s => s.SpeciesId == p.PredatorId)
                    ),
                    speRepo.ToDomain(
                        _context.Species.First(s => s.SpeciesId == p.PreyId)
                    )
                ))
                .ToList(),
            biome.UserId);

        return reconstructed;
    }
    
    public DateTime GetOldestBiomeEventDate(string biomeSlug)
    {
        var biomeId = _context.Biomes
                          .Where(b => b.Slug == biomeSlug)
                          .Select(b => (int?)b.BiomeId)
                          .FirstOrDefault()
                      ?? throw new InvalidOperationException("Biome inconnu.");

        var speciesMin = _context.BiomeHistory
            .Where(h => h.BiomeId == biomeId)
            .Select(h => (DateTime?)h.Timestamp)
            .Min();

        var predationMin = _context.PredationHistory
            .Where(h => h.BiomeId == biomeId)
            .Select(h => (DateTime?)h.Timestamp)
            .Min();

        return new[] { speciesMin, predationMin }
            .Where(d => d.HasValue)
            .Select(d => d!.Value)
            .DefaultIfEmpty(DateTime.UtcNow)
            .Min();
    }
    
    public IList<TimeEventsDto> GetAllEvents(string biomeSlug)
    {
        var biomeId = _context.Biomes
                          .Where(b => b.Slug == biomeSlug)
                          .Select(b => (int?)b.BiomeId)
                          .FirstOrDefault()
                      ?? throw new InvalidOperationException("Biome inconnu.");
        
        var speciesRaw = _context.BiomeHistory
            .Where(h => h.BiomeId == biomeId)
            .Include(h => h.Species)
            .ToList();

        var predationRaw = _context.PredationHistory
            .Where(h => h.BiomeId == biomeId)
            .Include(h => h.Predator)
            .Include(h => h.Prey)
            .ToList();

        var speciesEvents = speciesRaw.Select(h => new TimeEventsDto(
            h.Timestamp,
            $"{h.Species.Name} {(h.Delta > 0 ? "+" : "")}{h.Delta}"
        ));

        var predationEvents = predationRaw.Select(h => new TimeEventsDto(
            h.Timestamp,
            $"Predation {h.Predator.Name}->{h.Prey.Name} " +
            (h.Action == PredationAction.Added ? "ajoutée" : "retirée")
        ));

        return speciesEvents
            .Concat(predationEvents)
            .OrderBy(e => e.Timestamp)
            .ToList();
    }

    private Model.Biome ToInfrastructure(Biome biomeDomain, string userId)
    {
        return new Model.Biome
        {
            Name = biomeDomain.Name,
            Slug = biomeDomain.Name.ToKebabCase(),
            Temperature = Convert.ToDecimal(biomeDomain.Temperature),
            Humidity = Convert.ToDecimal(biomeDomain.Moisture),
            UserId = userId
        };
    }

    private Biome ToDomain(Model.Biome biomeInfra, SpeciesRepository speRepo, EquipmentRepository equipRepo)
    {
        var population = speRepo.GetPopulationForBiome(biomeInfra);
        var equipments = equipRepo.GetEquipmentForBiome(biomeInfra);

        var predations = biomeInfra.Predations?.Select(p => new Domains.Predation(
            speRepo.ToDomain(p.Predator),
            speRepo.ToDomain(p.Prey)
        )).ToList() ?? [];

        return new Biome(biomeInfra.Name,
            Convert.ToDouble(biomeInfra.Temperature),
            Convert.ToDouble(biomeInfra.Humidity),
            population, equipments, predations,
            biomeInfra.UserId);
    }

    private bool BiomeExists(Biome biome)
    {
        return _context.Biomes.Any(s => s.Slug.Equals(biome.Name.ToKebabCase()));
    }
}