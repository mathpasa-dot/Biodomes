using BioDomes.Domains;
using BioDomes.Domains.BiomesDom;
using BioDomes.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BioDomes.Web.Pages.Biomes;

[Authorize]
public class EditChain : PageModel
{
    private readonly IBiomeRepository _biomeRepo;

    public EditChain(IBiomeRepository biomeRepo)
    {
        _biomeRepo = biomeRepo;
    }
    
    public string BiomeDataJson { get; set; }

    public Biome Biome { get; set; }

    public IEnumerable<Domains.SpeciesDom.Species> SpeciesInBiome { get; set; }

    [BindProperty(SupportsGet = true)] public string Slug { get; set; }

    [BindProperty(SupportsGet = true)] public string? SelectedSpecies { get; set; }

    public IActionResult OnGet()
    {
        try
        {
            LoadBiome();

            SelectedSpecies ??= SpeciesInBiome.FirstOrDefault()?.Name;
        }
        catch (InvalidOperationException ex)
        {
            return NotFound();
        }

        return Page();
    }

    private void LoadBiome()
    {
        Biome = _biomeRepo.GetTargetBiome(Slug);
        SpeciesInBiome = Biome.Population.Keys;

        BiomeDataJson = new ChainService().JsonChain(Biome);
    }

    // Ajouter une nouvelle espèce
    public IActionResult OnPostUpdateSpeciesRelations(List<string> predators, List<string> prey)
    {
        try
        {
            var conflicts = new ChainService().GetConflictingSpecies(predators, prey);

            if (conflicts.Any())
            {
                ModelState.AddModelError(string.Empty,
                    $"Une espèce ne peut pas être à la fois prédateur et proie : {string.Join(", ", conflicts)}");

                LoadBiome();
                return Page();
            }

            _biomeRepo.UpdatePredationsForSpecies(
                Slug,
                SelectedSpecies.ToKebabCase(),
                predators.Select(p => p.ToKebabCase()).ToList(),
                prey.Select(p => p.ToKebabCase()).ToList()
            );
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }

        return RedirectToPage( new { slug = Slug });
    }
}