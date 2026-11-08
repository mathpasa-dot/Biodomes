#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using BioDomes.Infrastructures.Model;
using BioDomes.Web.ValidationRules;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace BioDomes.Web.Pages.Identity;

public class RegisterModel : PageModel
{
    private readonly IEmailSender _emailSender;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IDataProtector _protector;
    private readonly UserManager<User> _userManager;

    public RegisterModel(
        UserManager<User> userManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender,
        IWebHostEnvironment env,
        IDataProtectionProvider dataProtectionProvider)
    {
        _userManager = userManager;
        _logger = logger;
        _emailSender = emailSender;
        _env = env;
        _protector = dataProtectionProvider.CreateProtector("BioDomes.PendingRegistration");
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public void OnGet(string returnUrl = null)
    {
        ReturnUrl = returnUrl;
        Input = new InputModel { CurrentAvatarPath = "/images/avatars/default.jpeg" };
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        
        if (!ModelState.IsValid)
            return Page();

        // Refuser si l'email est déjà pris par un compte confirmé
        var existing = await _userManager.FindByEmailAsync(Input.Email);
        if (existing != null && await _userManager.IsEmailConfirmedAsync(existing))
        {
            ModelState.AddModelError(string.Empty, "Un compte existe déjà avec cette adresse email.");
            return Page();
        }

        var tempAvatarPath = await SaveTempImage();

        var pending = new PendingRegistration(
            Input.UserName, Input.Email, Input.Password,
            Input.ResearchTeam, Input.DateOfBirth!.Value, tempAvatarPath);

        var json = JsonSerializer.Serialize(pending);
        var token = WebEncoders.Base64UrlEncode(_protector.Protect(Encoding.UTF8.GetBytes(json)));

        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            null,
            new { area = "Identity", token, returnUrl },
            Request.Scheme);

        // Sans serveur SMTP configuré (démo), on confirme directement l'inscription.
        var smtpHost = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["SmtpSettings:Host"];
        if (string.IsNullOrWhiteSpace(smtpHost) && callbackUrl != null)
            return Redirect(callbackUrl);

        await _emailSender.SendEmailAsync(Input.Email, "Confirmez votre inscription",
            $"Confirmez votre compte BioDomes en <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>cliquant ici</a>.");

        _logger.LogInformation("Pending registration token sent to {Email}.", Input.Email);
        return RedirectToPage("/Identity/EmailConfirmation", new { type = "register-sent" });
    }

    private async Task<string?> SaveTempImage()
    {
        if (Input.UserAvatar == null || Input.UserAvatar.Length == 0) return null;

        var tempFolder = Path.Combine(_env.WebRootPath, "images", "avatars", "temp");
        Directory.CreateDirectory(tempFolder);

        var extension = Path.GetExtension(Input.UserAvatar.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(tempFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await Input.UserAvatar.CopyToAsync(stream);

        return $"/images/avatars/temp/{fileName}";
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Ce champ est requis.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ\s\-]+$", ErrorMessage = "Le nom ne peut contenir que des lettres et des espaces.")]
        [Display(Name = "Nom")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Ce champ est requis.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide."), ValidEmail]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ce champ est requis.")]
        [StringLength(100, ErrorMessage = "Le mot de passe doit être entre 6 et 100 caractères.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Ce champ est requis.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "La confirmation ne correspond pas au mot de passe.")]
        [Display(Name = "Confirmation")]
        public string ConfirmPassword { get; set; }
        
        [Required(ErrorMessage = "La date de naissance est requise.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date de naissance")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Equipe de recherche")] public string ResearchTeam { get; set; } = string.Empty;
        
        [ValidFile]
        public IFormFile? UserAvatar { get; set; }
        public string? CurrentAvatarPath { get; set; }
    }
}

internal record PendingRegistration(
    string UserName,
    string Email,
    string Password,
    string ResearchTeam,
    DateTime DateOfBirth,
    string? TempAvatarPath);