using System.Text.Json;
using BioDomes.Domains.BiomesDom;
using BioDomes.Domains.Dto;
using BioDomes.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Biome = BioDomes.Domains.BiomesDom.Biome;

namespace BioDomes.Web.Pages.Biomes;

public class DashboardModel : PageModel
{
    private readonly IBiomeRepository _biomeRepo;

    public DashboardModel(IBiomeRepository biomeRepo)
    {
        _biomeRepo = biomeRepo;
    }

    [BindProperty(SupportsGet = true)] public string Slug { get; set; } = string.Empty;
    public Biome Biome { get; set; } = null!;
    public ScoreDto Score { get; set; }
    public IList<HungrySpeciesWarning> BalanceComments { get; set; } = new List<HungrySpeciesWarning>();
    public string BiomeDataJson { get; set; } = "{}";
    
    public DateTime MinDate { get; set; }
    public DateTime MaxDate { get; set; }
    
    public IList<TimeEventsDto> LineComments { get; set; }

    public IActionResult OnGet()
    {
        try
        {
            Biome = _biomeRepo.GetTargetBiome(Slug);
            MinDate = _biomeRepo.GetOldestBiomeEventDate(Slug);
            MaxDate = DateTime.UtcNow;
            LineComments = _biomeRepo.GetAllEvents(Slug);
        } catch (InvalidOperationException ex)
        {
            return NotFound();
        }

        var qm = new QualityManager();
        Score = qm.ComputeScore(Biome);
        BalanceComments = qm.IsBiomeInBalance(Biome);

        BiomeDataJson = JsonSerializer.Serialize(new ChainService().JsonDashboard(Biome, Score, MinDate, 
                MaxDate, LineComments, BalanceComments), new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        });
        
        return Page();
    }
    
    public IActionResult OnGetBiomeState(DateTime date)
    {
        var utcDate = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        try
        {
            Biome = _biomeRepo.GetBiomeAtDate(Slug, utcDate);
            MinDate = _biomeRepo.GetOldestBiomeEventDate(Slug);
            MaxDate = DateTime.UtcNow;
            LineComments = _biomeRepo.GetAllEvents(Slug);
        } catch (InvalidOperationException ex)
        {
            return NotFound();
        }

        var qm = new QualityManager();
        Score = qm.ComputeScore(Biome);
        BalanceComments = qm.IsBiomeInBalance(Biome);
        
        return new JsonResult(new ChainService().JsonDashboard(Biome, Score, MinDate, 
            MaxDate, LineComments, BalanceComments));
    }
}
