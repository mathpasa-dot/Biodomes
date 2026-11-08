using BioDomes.Domains.BiomesDom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Biome = BioDomes.Domains.BiomesDom.Biome;
using Equipment = BioDomes.Domains.EquipmentsDom.Equipment;
using Predation = BioDomes.Domains.Predation;
using User = BioDomes.Infrastructures.Model.User;

namespace BioDomes.Web.Pages.Biomes;

[Authorize]
public class AddBiomeCommonModel : BiomeCommonForm
{
    private readonly UserManager<User> _userManager;

    public AddBiomeCommonModel(IBiomeRepository biomeRepository,
        UserManager<User> userManager) : base(biomeRepository)
    {
        _userManager = userManager;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        var newBiome = new Biome(Input.Name, Input.Temperature!.Value, Input.Humidity!.Value,
            new Dictionary<Domains.SpeciesDom.Species, int>(), new Dictionary<Equipment, int>(), new List<Predation>());

        var userId = _userManager.GetUserId(User)!;

        try
        {
            Repository.AddOne(newBiome, userId);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("Input.Name", ex.Message);
            return Page();
        }

        return RedirectToPage("/Biomes/Index");
    }
}