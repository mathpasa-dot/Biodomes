namespace BioDomes.Infrastructures.Model;

public class Equipment
{
    public int EquipmentId { get; set; }
    
    public string Slug { get; set; }

    public string Name { get; set; }

    public string? ImagePath { get; set; }
    public bool IsAvailable { get; set; } = false;
    public string UserId { get; set; }
    public User User { get; set; }

    public ICollection<EquipmentMaterial> EquipmentMaterials { get; set; }

    public IEnumerable<EquipmentMaterial> ConsumedMaterials
        => EquipmentMaterials?.Where(m => m.FlowType == MaterialFlowType.Consumed);

    public IEnumerable<EquipmentMaterial> ProducedMaterials
        => EquipmentMaterials?.Where(m => m.FlowType == MaterialFlowType.Produced);

    public List<BiomeEquipment> BiomeEquipments { get; set; } = new();
}