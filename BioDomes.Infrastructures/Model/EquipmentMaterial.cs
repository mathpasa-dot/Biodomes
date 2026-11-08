namespace BioDomes.Infrastructures.Model;

public enum MaterialFlowType
{
    Consumed = 0,
    Produced = 1
}

public class EquipmentMaterial
{
    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; }

    public int MaterialId { get; set; }
    public Material Material { get; set; }

    public MaterialFlowType FlowType { get; set; }
}