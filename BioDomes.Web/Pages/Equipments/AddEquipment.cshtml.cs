using BioDomes.Domains.EquipmentsDom;
using BioDomes.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Equipment = BioDomes.Domains.EquipmentsDom.Equipment;
using User = BioDomes.Infrastructures.Model.User;

namespace BioDomes.Web.Pages.Equipments;

[Authorize]
public class AddEquipmentModel : CommonForm
{
    private readonly UserManager<User> _userManager;

    public AddEquipmentModel(IEquipmentRepository equipmentRepository,
        UserManager<User> userManager, FileService imageService) : base(equipmentRepository, imageService)
    {
        _userManager = userManager;
    }

    public IActionResult OnGet()
    {
        LoadMaterial();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        LoadMaterial();

        if (!ModelState.IsValid)
            return Page();

        var equipment = new Equipment(Input.Name, Input.Produced.Select(p => new Material(p)).ToList(), 
            Input.Consumed.Select(p => new Material(p)).ToList(), ImageService.ComputeNewFileName(Input.ImageFile, Input.Name, "equipments"));

        var userId = _userManager.GetUserId(User)!;

        try
        {
            Repository.AddOne(equipment, userId);
            if (Input.ImageFile != null)
            {
                await ImageService.SaveImage(Input.ImageFile, Input.Name, Input.ExistingImagePath, "equipments");
            }
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("Input.Name", ex.Message);
            return Page();
        }

        return RedirectToPage("/Equipments/Index");
    }
}