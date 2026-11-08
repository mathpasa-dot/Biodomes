using System.Text.RegularExpressions;
using BioDomes.Domains.EquipmentsDom;
using BioDomes.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BioDomes.Web.Pages.Equipments;

public abstract class CommonForm : PageModel
{
    protected readonly IEquipmentRepository Repository;
    protected readonly FileService ImageService;

    protected CommonForm(IEquipmentRepository repository, FileService imageService)
    {
        Repository = repository;
        ImageService = imageService;
    }

    [BindProperty] public EquipmentInputModel Input { get; set; } = new();

    public IEnumerable<Material> Ingredients { get; private set; } = new List<Material>();

    protected void LoadMaterial()
    {
        Ingredients = Repository.GetAllMaterial();
    }
}