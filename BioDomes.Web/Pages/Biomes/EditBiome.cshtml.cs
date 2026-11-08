using BioDomes.Domains;
using BioDomes.Domains.BiomesDom;
using BioDomes.Domains.EquipmentsDom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioDomes.Web.Pages.Biomes;

[Authorize]
public class EditBiomeCommonModel : BiomeCommonForm
{
    public EditBiomeCommonModel(IBiomeRepository biomeRepository) : base(
        biomeRepository)
    {
    }

    [BindProperty(SupportsGet = true)] public string Slug { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        try
        {
            var biome = Repository.GetTargetBiome(Slug);
            
            Input.Name = biome.Name;
            Input.Temperature = biome.Temperature;
            Input.Humidity = biome.Moisture;

            return Page();
        } catch (InvalidOperationException ex)
        {
            return NotFound();
        }
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();
        
        try
        {
            var oldBiome = Repository.GetTargetBiome(Slug);

            var newBiome = new Biome(Input.Name, Input.Temperature!.Value, Input.Humidity!.Value,
                new Dictionary<Domains.SpeciesDom.Species, int>(), new Dictionary<Equipment, int>(), new List<Predation>());
            
            Repository.UpdateOne(oldBiome, newBiome);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }

        return RedirectToPage("/Biomes/Index");
    }
}