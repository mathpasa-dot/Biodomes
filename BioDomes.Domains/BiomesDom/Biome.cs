using BioDomes.Domains.EquipmentsDom;
using BioDomes.Domains.SpeciesDom;

namespace BioDomes.Domains.BiomesDom;

public record Biome(
    string Name,
    double Temperature,
    double Moisture,
    Dictionary<Species, int> Population,
    Dictionary<Equipment, int> Equipments,
    IReadOnlyList<Predation> Predations,
    string? OwnerId = null)
{
}