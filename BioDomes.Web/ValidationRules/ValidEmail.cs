using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace BioDomes.Web.ValidationRules;

public class ValidEmail : ValidationAttribute
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        var email = value.ToString()!;
        var members = new[] { validationContext.MemberName ?? "?" };

        if (!EmailRegex.IsMatch(email))
            return new ValidationResult(
                "L'adresse email n'est pas valide. Format attendu : utilisateur@domaine.tld",
                members);

        var localPart = email.Split('@')[0];
        if (localPart.Length > 64)
            return new ValidationResult(
                "La partie locale de l'email (avant @) ne peut pas dépasser 64 caractères.",
                members);

        if (email.Length > 254)
            return new ValidationResult(
                "L'adresse email ne peut pas dépasser 254 caractères.",
                members);

        var domain = email.Split('@')[1];
        try
        {
            var addresses = Dns.GetHostAddresses(domain);
            if (addresses.Length == 0)
                return new ValidationResult("Le domaine de l'email n'existe pas.", members);
        }
        catch (Exception ex) when (ex is SocketException or ArgumentException)
        {
            return new ValidationResult("Le domaine de l'email n'existe pas.", members);
        }

        return ValidationResult.Success;
    }
}