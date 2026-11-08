using BioDomes.Domains.SpeciesDom;
using BioDomes.Infrastructures.Model;
using BioDomes.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BioDomes.Web.Pages.Species;

[Authorize]
public class AddSpecieModel : CommonForm
{
    private readonly UserManager<User> _userManager;

    public AddSpecieModel(ISpeciesRepository speciesRepository,
        UserManager<User> userManager, FileService imageService) : base(speciesRepository, imageService)
    {
        _userManager = userManager;
    }

    public IActionResult OnGet()
    {
        LoadSelections();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        LoadSelections();

        if (!ModelState.IsValid)
            return Page();

        var newSpecies = new Domains.SpeciesDom.Species(Input.Name, Input.SpeciesType, Input.SpeciesRegime,
            Input.Height!.Value, Input.Weight!.Value, ImageService.ComputeNewFileName(Input.ImageFile, Input.Name, "species"));

        var userId = _userManager.GetUserId(User)!;

        try
        {
            Repository.AddOne(newSpecies, userId);
            if (Input.ImageFile != null)
            {
                await ImageService.SaveImage(Input.ImageFile, Input.Name, Input.ExistingImagePath, "species");
            }
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("Input.Name", ex.Message);
            return Page();
        }

        return RedirectToPage("/Species/Index");
    }
}