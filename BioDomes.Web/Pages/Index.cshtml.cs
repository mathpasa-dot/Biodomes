using BioDomes.Domains;
using BioDomes.Domains.BiomesDom;
using BioDomes.Domains.Dto;
using BioDomes.Domains.EquipmentsDom;
using BioDomes.Domains.SpeciesDom;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BioDomes.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IBiomeRepository _biomeRepository;
    private readonly ISpeciesRepository _speRepository;
    private readonly IEquipmentRepository _equipRepository;

    public IndexModel(IBiomeRepository biomeRepository,
        ISpeciesRepository speRepository, IEquipmentRepository equipRepository)
    {
        _biomeRepository = biomeRepository;
        _speRepository = speRepository;
        _equipRepository = equipRepository;
    }
    
    public IList<BiomeCardModel> BestBiomes { get; set; } = new List<BiomeCardModel>();

    public int ActiveCount { get; set; } = 24;

    public int SpeciesCount { get; set; } = 130;

    public int EquipCount { get; set; } = 12;

    public int AverageScore { get; set; } = 87;

    public void OnGet()
    {
        ActiveCount = _biomeRepository.Count();
        SpeciesCount =  _speRepository.Count();
        EquipCount = _equipRepository.Count();
        
        BiomeLookup(3);
    }

    private void BiomeLookup(int count)
    {
        BestBiomes = _biomeRepository.GetBestBiome(count);

        // Calcul moyenne
        if (BestBiomes.Count > 0)
        {
            int avg = 0;
            foreach (var b in BestBiomes)
            {
                avg += b.Score;
            } 
            AverageScore = avg / BestBiomes.Count;
        }
        else
        {
            AverageScore = 100;
        }
    }
}