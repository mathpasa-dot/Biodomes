using BioDomes.Domains.BiomesDom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using User = BioDomes.Infrastructures.Model.User;

namespace BioDomes.Web.Pages.Biomes;

[Authorize]
public class BiomeModel : PageModel
{
    private readonly IBiomeRepository _biomeRepository;
    private readonly UserManager<User> _userManager;

    public BiomeModel(IBiomeRepository biomeRepository, UserManager<User> userManager)
    {
        _biomeRepository = biomeRepository;
        _userManager = userManager;
    }

    public IEnumerable<BiomeCardViewModel> Biomes { get; private set; } = [];

    public void OnGet()
    {
        var userId = _userManager.GetUserId(User)!;
        var biomes = User.IsInRole("Admin")
            ? _biomeRepository.GetAll()
            : _biomeRepository.GetAllForUser(userId);
        
        var quality = new QualityManager();
        
        Biomes = biomes.Select(b => new BiomeCardViewModel
        {
            Biome = b,
            CanManage = b.OwnerId == userId || User.IsInRole("Admin"),
            Score = quality.ComputeScore(b).Total,
            State = @FactorManager.TranslateTemperatureToState(b.Temperature),
            IsBalanced = quality.IsBiomeInBalance(b).Count == 0,
            TotalSpePop = b.Population.Values.Sum(),
            TotalEquipPop = b.Equipments.Values.Sum()
        });
    }

    public IActionResult OnPostDelete(string slug)
    {
        try
        {
            _biomeRepository.RemoveOne(slug);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        
        return RedirectToPage("/Biomes/Index");
    }
}

public class BiomeCardViewModel
{
    public Biome Biome { get; set; }
    public bool CanManage { get; set; }
    public int Score { get; set; }
    
    public BiomeState State { get; set; }
    public bool IsBalanced { get; set; }
    public int TotalSpePop { get; set; }
    public int TotalEquipPop { get; set; }
}