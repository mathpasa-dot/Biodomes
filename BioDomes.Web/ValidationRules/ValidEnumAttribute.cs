using System.ComponentModel.DataAnnotations;

namespace BioDomes.Web.ValidationRules;

public class ValidEnumAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public ValidEnumAttribute (Type enumType)
    {
        _enumType = enumType;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        if (!Enum.IsDefined(_enumType, value))
        {
            return new ValidationResult($"Valeur invalide pour {_enumType.Name}.");
        }

        return ValidationResult.Success;
    }
}