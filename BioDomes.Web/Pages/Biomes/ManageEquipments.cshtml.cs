using Microsoft.AspNetCore.Mvc.RazorPages;
using BioDomes.Domains;
using BioDomes.Domains.BiomesDom;
using BioDomes.Domains.EquipmentsDom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Biome = BioDomes.Domains.BiomesDom.Biome;
using Equipment = BioDomes.Domains.EquipmentsDom.Equipment;
using User = BioDomes.Infrastructures.Model.User;

namespace BioDomes.Web.Pages.Biomes;

[Authorize]
public class ManageEquipments : PageModel
{
    private readonly IBiomeRepository _biomeRepo;
    private readonly IEquipmentRepository _equipmentRepo;
    private readonly UserManager<User> _userManager;

    public ManageEquipments(IBiomeRepository biomeRepo,
        IEquipmentRepository equipmentRepo, UserManager<User> userManager)
    {
        _biomeRepo = biomeRepo;
        _equipmentRepo = equipmentRepo;
        _userManager = userManager;
    }

    public Biome Biome { get; set; }

    public IEnumerable<Equipment> AvailableEquipToAdd { get; set; }
    
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
    public IActionResult OnPostUpdateEquipQuantities(Dictionary<string, int> quantities)
    {
        if (!TryLoadBiome())
            return NotFound();
        
        try
        {
            foreach (var entry in quantities) _biomeRepo.AddEquipToBiome(Slug, entry.Key.ToKebabCase(), entry.Value);
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
    public IActionResult OnPostAddEquip(string equipName, int quantity)
    {
        if (!TryLoadBiome())
            return NotFound();
        
        try
        {
            _biomeRepo.AddEquipToBiome(Slug, equipName.ToKebabCase(), quantity);
            SuccessMessage = $"Equipement \"{equipName}\" ajoutée avec succès.";
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

    public IActionResult OnPostDeleteEquip(string equipName)
    {
        if (!TryLoadBiome())
            return NotFound();
        
        try
        {
            _biomeRepo.RemoveEquipFromBiome(Slug, equipName.ToKebabCase());
            SuccessMessage = $"Equipement \"{equipName}\" supprimée avec succès.";
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

        var allEquipments = User.IsInRole("Admin")
            ? _equipmentRepo.GetAll()
            : _equipmentRepo.GetAvailableAndOwn(userId);

        AvailableEquipToAdd = allEquipments
            .Where(s => !Biome.Equipments.Keys.Any(b => b.Name == s.Name));
    }
}