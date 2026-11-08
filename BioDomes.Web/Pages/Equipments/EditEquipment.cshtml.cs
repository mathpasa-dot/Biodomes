using BioDomes.Domains.Dto;
using BioDomes.Domains.EquipmentsDom;
using BioDomes.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioDomes.Web.Pages.Equipments;

[Authorize]
public class EditEquipmentModel : CommonForm
{
    public EditEquipmentModel(IEquipmentRepository equipmentRepository,
        FileService imageService) : base(equipmentRepository, imageService)
    {}
    
    [BindProperty(SupportsGet = true)] public string Slug { get; set; } = string.Empty;
    
    public List<BiomeImpactDto> ImpactedBiomes { get; set; } = new();

    public IActionResult OnGet()
    {
        LoadMaterial();
        ImpactedBiomes = Repository.GetBiomesForEquipment(Slug);
        
        try
        {
            var equipment = Repository.GetTargetEquipment(Slug);
            
            Input.Name = equipment.Name;
            Input.Consumed = equipment.Consumed.Select(e => e.Name);
            Input.Produced = equipment.Produced.Select(e => e.Name);
            Input.ExistingImagePath = equipment.FilePath;

            return Page();
        } catch (InvalidOperationException ex)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        LoadMaterial();
        
        if (!ModelState.IsValid)
            return Page();
        
        try
        {
            var oldEquip = Repository
                .GetTargetEquipment(Slug);

            var newEquip = new Equipment(Input.Name, Input.Produced.Select(p => new Material(p)).ToList(), 
                Input.Consumed.Select(p => new Material(p)).ToList(), ImageService.ComputeNewFileName(Input.ImageFile, Input.Name, "equipments"));
        
            Repository.UpdateOne(oldEquip, newEquip);
            if (Input.ImageFile != null)
            {
                await ImageService.SaveImage(Input.ImageFile, Input.Name, Input.ExistingImagePath, "equipments");
            }
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }

        return RedirectToPage("/Equipments/Index");
    }
}