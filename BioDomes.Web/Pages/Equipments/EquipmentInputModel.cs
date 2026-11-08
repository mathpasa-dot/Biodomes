using System.ComponentModel.DataAnnotations;
using BioDomes.Web.ValidationRules;

namespace BioDomes.Web.Pages.Equipments;

public class EquipmentInputModel
{
    [Required(ErrorMessage = "Ce champ est requis.")]
    [Display(Name = "Nom")]
    [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ\s\-]+$", ErrorMessage = "Le nom ne peut contenir que des lettres et des espaces.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Production")]
    public IEnumerable<string> Produced { get; set; }

    [Display(Name = "Consommation")]
    public IEnumerable<string> Consumed { get; set; }

    [ValidFile]
    [Display(Name = "Image (facultatif)")]
    public IFormFile? ImageFile { get; set; }

    public string? ExistingImagePath { get; set; }
}