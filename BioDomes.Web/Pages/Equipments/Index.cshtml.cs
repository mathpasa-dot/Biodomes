using BioDomes.Domains;
using BioDomes.Domains.Dto;
using BioDomes.Domains.EquipmentsDom;
using BioDomes.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using User = BioDomes.Infrastructures.Model.User;

namespace BioDomes.Web.Pages.Equipments;

[Authorize]
public class EquipmentModel : PageModel
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly FileService _imageService;
    private readonly UserManager<User> _userManager;

    public EquipmentModel(IEquipmentRepository equipmentRepository, UserManager<User> userManager, FileService imageService)
    {
        _equipmentRepository = equipmentRepository;
        _imageService = imageService;
        _userManager = userManager;
    }
    
    public IEnumerable<EquipmentCardViewModel> Equipments { get; private set; } = [];
    
    public Dictionary<string, List<BiomeImpactDto>> EquipBiomeImpacts { get; set; } = new();

    public void OnGet()
    {
        var userId = _userManager.GetUserId(User)!;
        var equipments = User.IsInRole("Admin")
            ? _equipmentRepository.GetAll()
            : _equipmentRepository.GetAvailableAndOwn(userId);
        
        Equipments = equipments.Select(e => new EquipmentCardViewModel
        {
            Equipment = e,
            IsOwner = e.OwnerId == userId,
            IsAdmin = User.IsInRole("Admin")
        });
        
        foreach (var equipment in equipments)
        {
            var slug = equipment.Name.ToKebabCase();
            EquipBiomeImpacts[slug] = _equipmentRepository.GetBiomesForEquipment(slug);
        }
    }
    
    public IActionResult OnPostDelete(string slug)
    {
        try
        {
            _imageService.DeleteEquipImage(slug);
            _equipmentRepository.RemoveOne(slug);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        
        return RedirectToPage("/Equipments/Index");
    }

    public IActionResult OnPostValidate(string slug)
    {
        try
        {
            _equipmentRepository.MakeAvailable(slug);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        
        return RedirectToPage();
    }
}

public class EquipmentCardViewModel
{
    public Equipment Equipment { get; set; }
    public bool IsOwner { get; set; }
    public bool IsAdmin { get; set; }
}