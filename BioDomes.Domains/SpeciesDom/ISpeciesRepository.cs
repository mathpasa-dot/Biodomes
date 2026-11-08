using BioDomes.Domains.Dto;

namespace BioDomes.Domains.SpeciesDom;

public interface ISpeciesRepository
{
    int Count();
    
    // Toutes les espèces (admin uniquement)
    IList<Species> GetAll();

    // Espèces disponibles + propres
    IList<Species> GetAvailableAndOwn(string userId);
    
    Species GetTargetSpecies(string slug);

    void AddOne(Species species, string userId);

    void UpdateOne(Species oldSpecies, Species newSpecies);

    void RemoveOne(string slug);

    // Valider une espèce (admin)
    void MakeAvailable(string slug);
    
    List<BiomeImpactDto> GetBiomesForSpecies(string slug);
}