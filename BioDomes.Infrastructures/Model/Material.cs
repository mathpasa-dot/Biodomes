namespace BioDomes.Infrastructures.Model;

public class Material
{
    public int MaterialId { get; set; }
    
    public string Slug { get; set; }
    public string Name { get; set; }

    public List<EquipmentMaterial> EquipmentMaterials { get; set; } = new();
}