namespace BioDomes.Domains.EquipmentsDom;

public record Equipment(string Name, IList<Material> Produced, IList<Material> Consumed, string? FilePath = null)
{
    public bool IsAvailable { get; set; }
    public string? OwnerId { get; set; }
}