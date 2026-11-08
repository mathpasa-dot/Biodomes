using System.ComponentModel.DataAnnotations;
using BioDomes.Domains.SpeciesDom;
using BioDomes.Web.ValidationRules;

namespace BioDomes.Web.Pages.Species;

public class SpeciesInputModel : IValidatableObject
{
    [Display(Name = "Nom")]
    [Required(ErrorMessage = "Ce champ est requis.")]
    [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ\s\-]+$", ErrorMessage = "Le nom ne peut contenir que des lettres et des espaces.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Type")]
    [ValidEnum(typeof(SpeciesType))]
    [Required(ErrorMessage = "Ce champ est requis.")] public SpeciesType SpeciesType { get; set; }

    [Display(Name = "Régime alimentaire")]
    [ValidEnum(typeof(SpeciesRegime))]
    [Required(ErrorMessage = "Ce champ est requis.")] public SpeciesRegime SpeciesRegime { get; set; }

    [Display(Name = "Taille (cm)")]
    [Required(ErrorMessage = "Ce champ est requis.")]
    [Range(0, 10000, ErrorMessage = "La taille doit être comprise entre 0 et 10000 cm.")]
    public int? Height { get; set; }

    [Display(Name = "Poid (kg)")]
    [Required(ErrorMessage = "Ce champ est requis.")]
    [Range(0.0, double.MaxValue, ErrorMessage = "Le poids doit être positif.")]
    public double? Weight { get; set; }

    [ValidFile]
    [Display(Name = "Image (facultatif)")]
    public IFormFile? ImageFile { get; set; }

    public string? ExistingImagePath { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!SpeciesCompatibility.IsRegimeCompatible(SpeciesType, SpeciesRegime))
        {
            yield return new ValidationResult(
                $"Le régime \"{SpeciesRegime.ToDisplayString()}\" n'est pas compatible avec le type \"{SpeciesType.ToDisplayString()}\".",
                [nameof(SpeciesRegime)]);
        }
    }
}