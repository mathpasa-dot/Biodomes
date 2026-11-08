using System.ComponentModel.DataAnnotations;
using BioDomes.Domains.EquipmentsDom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BioDomes.Web.Pages.Materials;

[Authorize(Roles = "Admin")]
public class AddMaterialModel : PageModel
{
    private readonly IEquipmentRepository _repository;

    public AddMaterialModel(IEquipmentRepository repository)
    {
        _repository = repository;
    }

    public IList<Material> Materials { get; private set; } = new List<Material>();

    [BindProperty] [Required(ErrorMessage = "Ce champ est requis.")] public string Name { get; set; } = string.Empty;

    public void OnGet()
    {
        Materials = _repository.GetAllMaterial();
    }

    public IActionResult OnPost()
    {
        Materials = _repository.GetAllMaterial();

        if (!ModelState.IsValid) return Page();

        try
        {
            _repository.AddOneMaterial(Name.Trim());
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("Name", ex.Message);
            return Page();
        }
        
        return RedirectToPage("/Index");
    }
}