using System.ComponentModel.DataAnnotations;

namespace BioDomes.Web.ValidationRules;

public class ValidFile : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext
        validationContext)
    {
        if (value == null) return ValidationResult.Success;

        if (value is not IFormFile file)
            return new ValidationResult("Should be used against IFormFile",
                new[] { validationContext.MemberName ?? "?" });

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            return new ValidationResult("Only .jpg, .jpeg, .png and .webp images are allowed.",
                new[] { validationContext.MemberName ?? "?" });

        const long maxBytes = 5 * 1024 * 1024; // 5 MB
        if (file.Length > maxBytes)
            return new ValidationResult("Image must be smaller than 5 MB.",
                new[] { validationContext.MemberName ?? "?" });

        using var fileStream = file.OpenReadStream();
        var fileContent = new byte[4];
        var bytesRead = fileStream.Read(fileContent, 0, 4);
        if (bytesRead < 4)
            return new ValidationResult("Expecting at least 4 bytes got " +
                                        bytesRead, new[] { validationContext.MemberName ?? "?" });

        return ValidationResult.Success;
    }
}