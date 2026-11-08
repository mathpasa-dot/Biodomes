using BioDomes.Domains;
using BioDomes.Domains.BiomesDom;
using BioDomes.Domains.Dto;
using BioDomes.Domains.SpeciesDom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Biome = BioDomes.Domains.BiomesDom.Biome;
using User = BioDomes.Infrastructures.Model.User;

namespace BioDomes.Web.Pages.Biomes;

[Authorize]
public class ManageSpecies : PageModel
{
    private readonly IBiomeRepository _biomeRepo;
    private readonly ISpeciesRepository _speciesRepo;
    private readonly UserManager<User> _userManager;

    public ManageSpecies(IBiomeRepository biomeRepo, ISpeciesRepository speciesRepo, UserManager<User> userManager)
    {
        _biomeRepo = biomeRepo;
        _speciesRepo = speciesRepo;
        _userManager = userManager;
    }

    public Biome Biome { get; set; }

    public IEnumerable<Domains.SpeciesDom.Species> AvailableSpeciesToAdd { get; set; }

    public IList<HungrySpeciesWarning> HungryWarnings { get; set; } = [];

    public IList<string> Suggestions { get; set; } = [];

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    [BindProperty(SupportsGet = true)] public string Slug { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        if (!TryLoadBiome())
            return NotFound();

        LoadData();
        return Page();
    }

    // Modifier les quantités des espèces existantes
    public IActionResult OnPostUpdateSpeciesQuantities(Dictionary<string, int> quantities)
    {
        if (!TryLoadBiome())
            return NotFound();
        
        try
        {
            foreach (var entry in quantities) _biomeRepo.AddSpeciesToBiome(Slug, entry.Key.ToKebabCase(), entry.Value);
            SuccessMessage = "Quantités ajustées avec succès.";
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            LoadData();
            ErrorMessage = ex.Message;
            return Page();
        }

        return RedirectToPage(new { slug = Slug });
    }

    // Ajouter une nouvelle espèce
    public IActionResult OnPostAddSpecies(string speciesName, int quantity,
        List<string> predators, List<string> prey)
    {
        if (!TryLoadBiome())
            return NotFound();
        
        try
        {
            _biomeRepo.AddSpeciesToBiome(Slug, speciesName.ToKebabCase(), quantity);

            foreach (var predator in predators)
                _biomeRepo.AddPredation(Slug, predator.ToKebabCase(), speciesName.ToKebabCase());

            foreach (var p in prey)
                _biomeRepo.AddPredation(Slug, speciesName.ToKebabCase(), p.ToKebabCase());
            
            SuccessMessage = $"Espèce \"{speciesName}\" ajoutée avec succès.";
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            LoadData();
            ErrorMessage = ex.Message;
            return Page();
        }

        return RedirectToPage(new { slug = Slug });
    }

    public IActionResult OnPostDeleteSpecies(string speciesName)
    {
        if (!TryLoadBiome())
            return NotFound();
        
        try
        {
            _biomeRepo.RemoveSpeciesFromBiome(Slug, speciesName.ToKebabCase());
            SuccessMessage = $"Espèce \"{speciesName}\" supprimée avec succès.";
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            LoadData();
            ErrorMessage = ex.Message;
            return Page();
        }

        return RedirectToPage(new { slug = Slug });
    }
    
    private bool TryLoadBiome()
    {
        try
        {
            Biome = _biomeRepo.GetTargetBiome(Slug);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
    
    private void LoadData()
    {
        var userId = _userManager.GetUserId(User)!;

        var allSpecies = User.IsInRole("Admin")
            ? _speciesRepo.GetAll()
            : _speciesRepo.GetAvailableAndOwn(userId);

        AvailableSpeciesToAdd = allSpecies
            .Where(s => !Biome.Population.Keys.Any(b => b.Name == s.Name));
        
        QualityManager qm = new QualityManager();

        HungryWarnings = qm.IsBiomeInBalance(Biome);
        
        Suggestions = qm.GenerateSuggestions(Biome, HungryWarnings);
    }
}