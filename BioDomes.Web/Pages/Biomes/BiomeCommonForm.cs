using BioDomes.Domains.BiomesDom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BioDomes.Web.Pages.Biomes;

public class BiomeCommonForm : PageModel
{
    protected readonly IBiomeRepository Repository;

    protected BiomeCommonForm(IBiomeRepository repository)
    {
        Repository = repository;
    }

    [BindProperty] public BiomeInputModel Input { get; set; } = new();
}