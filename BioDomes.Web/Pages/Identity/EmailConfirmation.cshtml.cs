using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BioDomes.Web.Pages.Identity;

public class EmailConfirmationModel : PageModel
{
    public string Message { get; set; } = string.Empty;

    public IActionResult OnGet(string type)
    {
        Message = type switch
        {
            "register-sent" => "Un email de vérification vous a été envoyé. Cliquez sur le lien reçu pour activer votre compte.",
            "register"      => "Votre inscription a été validée !",
            "email-sent"    => "Un email de confirmation a été envoyé à votre nouvelle adresse.",
            "email"         => "Votre adresse mail a bien été modifiée !",
            _               => "Opération confirmée."
        };
        return Page();
    }
}