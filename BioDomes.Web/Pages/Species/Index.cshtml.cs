using BioDomes.Domains;
using BioDomes.Domains.Dto;
using BioDomes.Domains.SpeciesDom;
using BioDomes.Infrastructures.Model;
using BioDomes.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BioDomes.Web.Pages.Species;

[Authorize]
public class SpeciesModel : PageModel
{
    private readonly ISpeciesRepository _repository;
    private readonly FileService _imageService;
    private readonly UserManager<User> _userManager;

    public SpeciesModel(ISpeciesRepository repository, UserManager<User> userManager, FileService imageService)
    {
        _repository = repository;
        _userManager = userManager;
        _imageService = imageService;
    }
    
    public IEnumerable<SpeciesCardViewModel> Species { get; private set; } = Enumerable.Empty<SpeciesCardViewModel>();
    
    public Dictionary<string, List<BiomeImpactDto>> SpeciesBiomeImpacts { get; set; } = new();

    public void OnGet()
    {
        var userId = _userManager.GetUserId(User)!;
        var species = User.IsInRole("Admin")
            ? _repository.GetAll()
            : _repository.GetAvailableAndOwn(userId);
        
        Species = species.Select(s => new SpeciesCardViewModel
        {
            Species = s,
            IsOwner = s.OwnerId == userId,
            IsAdmin = User.IsInRole("Admin")
        });
        
        foreach (var spe in species)
        {
            var slug = spe.Name.ToKebabCase();
            SpeciesBiomeImpacts[slug] = _repository.GetBiomesForSpecies(slug);
        }
    }

    public IActionResult OnPostDelete(string slug)
    {
        try
        {
            _imageService.DeleteSpeciesImage(slug);
            _repository.RemoveOne(slug);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            OnGet();
            return Page();
        }
        
        return RedirectToPage("/Species/Index");
    }

    public IActionResult OnPostValidate(string slug)
    {
        try
        {
            _repository.MakeAvailable(slug);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        
        return RedirectToPage();
    }
}

public class SpeciesCardViewModel
{
    public Domains.SpeciesDom.Species Species { get; set; }
    public bool IsOwner { get; set; }
    public bool IsAdmin { get; set; }
}