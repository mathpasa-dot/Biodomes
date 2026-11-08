using BioDomes.Domains.Dto;

namespace BioDomes.Domains.EquipmentsDom;

public interface IEquipmentRepository
{
    int Count();
    
    //Tous les équipements (admin uniquement)
    IList<Equipment> GetAll();

    // Équipements disponibles + propres
    IList<Equipment> GetAvailableAndOwn(string userId);
    
    Equipment GetTargetEquipment(string slug);

    void AddOne(Equipment equipment, string userId);
    
    void UpdateOne(Equipment oldEquip, Equipment newEquip);

    void RemoveOne(string slug);

    // Valider un équipement (admin)
    void MakeAvailable(string slug);

    List<BiomeImpactDto> GetBiomesForEquipment(string slug);

    IList<Material> GetAllMaterial();

    void AddOneMaterial(string name);
}