using Microsoft.AspNetCore.Identity;

namespace BioDomes.Infrastructures.Model;

public class User : IdentityUser
{
    public DateTime DateOfBirth { get; set; }

    public string? ResearchTeam { get; set; } = string.Empty;

    public string? UserAvatar { get; set; } = string.Empty;

    public List<Species> Species { get; set; } = new();

    public List<Biome> Biomes { get; set; } = new();

    public List<Equipment> Equipments { get; set; } = new();
}