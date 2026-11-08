using System.ComponentModel.DataAnnotations;
using BioDomes.Domains.BiomesDom;

namespace BioDomes.Web.Pages.Biomes;

public class BiomeInputModel : IValidatableObject
{
    [Display(Name = "Nom")]
    [Required(ErrorMessage = "Ce champ est requis.")]
    [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ\s\-]+$", ErrorMessage = "Le nom ne peut contenir que des lettres et des espaces.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Température (C°)")]
    [Required(ErrorMessage = "Ce champ est requis.")]
    [Range(-200, 200, ErrorMessage = "La température doit être comprise entre -200 et 200 °C.")]
    public double? Temperature { get; set; }

    [Display(Name = "Humidité (g/m³)")]
    [Required(ErrorMessage = "Ce champ est requis.")]
    public double? Humidity { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Temperature.HasValue && Humidity.HasValue)
        {
            var error = FactorManager.Respect(Temperature.Value, Humidity.Value);
            if (error != null)
                yield return new ValidationResult(error, [nameof(Humidity)]);
        }
    }
}