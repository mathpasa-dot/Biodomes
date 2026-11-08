using System.Text;
using System.Text.Json;
using BioDomes.Infrastructures.Model;
using BioDomes.Web.Pages.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace BioDomes.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ConfirmEmailModel : PageModel
{
    private readonly IDataProtector _protector;
    private readonly IWebHostEnvironment _env;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public ConfirmEmailModel(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IWebHostEnvironment env,
        IDataProtectionProvider dataProtectionProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _env = env;
        _protector = dataProtectionProvider.CreateProtector("BioDomes.PendingRegistration");
    }

    public async Task<IActionResult> OnGetAsync(string token)
    {
        if (token == null)
            return RedirectToPage("/Index");

        PendingRegistration pending;
        try
        {
            var json = Encoding.UTF8.GetString(_protector.Unprotect(WebEncoders.Base64UrlDecode(token)));
            pending = JsonSerializer.Deserialize<PendingRegistration>(json)!;
        }
        catch
        {
            return RedirectToPage("/Index");
        }

        var user = new User
        {
            UserName     = pending.UserName,
            Email        = pending.Email,
            ResearchTeam = pending.ResearchTeam,
            DateOfBirth  = pending.DateOfBirth,
            UserAvatar   = MoveTempAvatar(pending.TempAvatarPath, pending.Email),
        };

        var result = await _userManager.CreateAsync(user, pending.Password);
        if (!result.Succeeded)
            return RedirectToPage("/Index");

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _userManager.ConfirmEmailAsync(user, code);
        await _userManager.AddToRoleAsync(user, "Employee");

        await _signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToPage("/Identity/EmailConfirmation", new { area = "", type = "register" });
    }

    private string? MoveTempAvatar(string? tempPath, string email)
    {
        if (tempPath == null) return null;

        var tempFull = Path.Combine(_env.WebRootPath, tempPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (!System.IO.File.Exists(tempFull)) return null;

        var extension = Path.GetExtension(tempFull);
        var finalName = $"{email}{extension}";
        var finalFull = Path.Combine(_env.WebRootPath, "images", "avatars", finalName);
        System.IO.File.Move(tempFull, finalFull, overwrite: true);

        return $"/images/avatars/{finalName}";
    }
}