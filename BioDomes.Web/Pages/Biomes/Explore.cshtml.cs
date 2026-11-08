using BioDomes.Domains.BiomesDom;
using BioDomes.Domains.EquipmentsDom;
using BioDomes.Domains.SpeciesDom;
using BioDomes.Infrastructures.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Equipment = BioDomes.Domains.EquipmentsDom.Equipment;

namespace BioDomes.Web.Pages.Biomes;

public class ExploreModel : PageModel
{
    private readonly IBiomeRepository _biomeRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly UserManager<User> _userManager;

    public ExploreModel(IBiomeRepository biomeRepository, ISpeciesRepository speciesRepository , 
        IEquipmentRepository equipmentRepository, UserManager<User> userManager)
    {
        _biomeRepository = biomeRepository;
        _speciesRepository = speciesRepository;
        _equipmentRepository = equipmentRepository;
        _userManager = userManager;
    }

    [BindProperty(SupportsGet = true)]
    public double? TempMin { get; set; }

    [BindProperty(SupportsGet = true)]
    public double? TempMax { get; set; }

    [BindProperty(SupportsGet = true)]
    public double? MoistureMin { get; set; }

    [BindProperty(SupportsGet = true)]
    public double? MoistureMax { get; set; }
    
    public bool HasFilters { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Species { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Equipment { get; set; }

    public IEnumerable<BiomeCardViewModel> Biomes { get; private set; } = [];
    
    public IList<Domains.SpeciesDom.Species> AllSpecies { get; private set; } = [];
    public IList<Equipment> AllEquipments { get; private set; } = [];

    public void OnGet()
    {
        var biomes = _biomeRepository.GetByFilter(TempMin,  TempMax, MoistureMin, MoistureMax, Species, Equipment);
        
        AllSpecies = _speciesRepository.GetAll();
        AllEquipments = _equipmentRepository.GetAll();

        HasFilters = TempMin != null || TempMax != null
             || MoistureMin != null || MoistureMax != null
             || !string.IsNullOrWhiteSpace(Species)
             || !string.IsNullOrWhiteSpace(Equipment);
        
        var userId = _userManager.GetUserId(User)!;
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
}