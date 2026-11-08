using BioDomes.Domains.Dto;
using BioDomes.Domains.SpeciesDom;
using BioDomes.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioDomes.Web.Pages.Species;

[Authorize]
public class EditSpeciesModel : CommonForm
{
    public EditSpeciesModel(ISpeciesRepository speciesRepository, FileService imageService) : base(
        speciesRepository, imageService)
    {
    }

    [BindProperty(SupportsGet = true)] public string Slug { get; set; } = string.Empty;
    
    public List<BiomeImpactDto> ImpactedBiomes { get; set; } = new();

    public IActionResult OnGet()
    {
        LoadSelections();
        
        ImpactedBiomes = Repository.GetBiomesForSpecies(Slug);
        try
        {
            var species = Repository.GetTargetSpecies(Slug);
            
            Input.Name = species.Name;
            Input.SpeciesType = species.SpeciesType;
            Input.SpeciesRegime = species.SpeciesRegime;
            Input.Height = species.Height;
            Input.Weight = species.Weight;
            Input.ExistingImagePath = species.FilePath;

            return Page();
        } catch (InvalidOperationException ex)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        LoadSelections();

        if (!ModelState.IsValid)
            return Page();
        
        try
        {
            var oldSpecies = Repository.GetTargetSpecies(Slug);

            var newSpecies = new Domains.SpeciesDom.Species(Input.Name, Input.SpeciesType, Input.SpeciesRegime,
                Input.Height!.Value, Input.Weight!.Value, ImageService.ComputeNewFileName(Input.ImageFile, Input.Name, "species"));
            
            Repository.UpdateOne(oldSpecies, newSpecies);
            if (Input.ImageFile != null)
            {
                await ImageService.SaveImage(Input.ImageFile, Input.Name, Input.ExistingImagePath, "species");
            }
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }

        return RedirectToPage("/Species/Index");
    }
}