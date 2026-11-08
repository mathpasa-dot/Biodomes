using BioDomes.Domains.Dto;

namespace BioDomes.Domains.BiomesDom;

public interface IBiomeRepository
{
    int Count();
    
    IList<Biome> GetAll();

    IList<Biome> GetAllForUser(string userId);
    
    Biome GetTargetBiome(string biomeSlug);

    IList<BiomeCardModel> GetBestBiome(int count);

    IList<Biome> GetByFilter(double? tempMin, double? tempMax, double? moistureMin, double? moistureMax,
        string? species, string? equipment);

    void AddOne(Biome biome, string userId);

    void UpdateOne(Biome oldBiome, Biome newBiome);

    void RemoveOne(string slug);

    void AddSpeciesToBiome(string biomeSlug, string speciesSlug, int count);
    
    void AddEquipToBiome(string biomeSlug, string equipSlug, int count);
    
    void RemoveSpeciesFromBiome(string biomeSlug, string speciesSlug);

    void RemoveEquipFromBiome(string biomeSlug, string equipSlug);
    
    void AddPredation(string biomeSlug, string predatorSlug, string preySlug);

    void UpdatePredationsForSpecies(string biomeSlug, string speciesSlug,
        List<string> newPredatorSlugs, List<string> newPreySlugs);

    Biome GetBiomeAtDate(string biomeSlug, DateTime targetDate);

    DateTime GetOldestBiomeEventDate(string biomeSlug);

    IList<TimeEventsDto> GetAllEvents(string biomeSlug);
}