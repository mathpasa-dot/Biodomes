using System.Text.RegularExpressions;
using BioDomes.Domains.SpeciesDom;
using BioDomes.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BioDomes.Web.Pages.Species;

public abstract class CommonForm : PageModel
{
    protected readonly FileService ImageService;
    protected readonly ISpeciesRepository Repository;

    protected CommonForm(ISpeciesRepository repository, FileService imageService)
    {
        Repository = repository;
        ImageService = imageService;
    }

    [BindProperty] public SpeciesInputModel Input { get; set; } = new();

    public IEnumerable<SpeciesType> SpeciesTypes { get; set; } = [];
    public IEnumerable<SpeciesRegime> Regimes { get; set; } = [];

    protected void LoadSelections()
    {
        SpeciesTypes = Enum.GetValues<SpeciesType>();
        Regimes = Enum.GetValues<SpeciesRegime>();
    }
}