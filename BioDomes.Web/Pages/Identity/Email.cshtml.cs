// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using BioDomes.Infrastructures.Model;
using BioDomes.Web.ValidationRules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace BioDomes.Web.Pages.Identity;

[Authorize]
public class EmailModel : PageModel
{
    private readonly IEmailSender _emailSender;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public EmailModel(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }
    
    public string Email { get; set; }
    
    public bool IsEmailConfirmed { get; set; }
    
    [TempData]
    public string StatusMessage { get; set; }
    
    [BindProperty]
    public InputModel Input { get; set; }

    private async Task LoadAsync(User user)
    {
        var email = await _userManager.GetEmailAsync(user);
        Email = email;

        Input = new InputModel
        {
            NewEmail = email
        };

        IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostChangeEmailAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var email = await _userManager.GetEmailAsync(user);
        if (Input.NewEmail != email)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmailChange",
                null,
                new { area = "Identity", userId, email = Input.NewEmail, code },
                Request.Scheme);

            await _emailSender.SendEmailAsync(Input.NewEmail, "Confirmez votre nouvelle adresse mail",
                $"Confirmez le changement d'adresse mail en <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>cliquant ici</a>.");

            return RedirectToPage("/Identity/EmailConfirmation", new { type = "email-sent" });
        }

        StatusMessage = "Your email is unchanged.";
        return RedirectToPage();
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Ce champ est requis.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide."), ValidEmail]
        [Display(Name = "Nouvel email")]
        public string NewEmail { get; set; }
    }
}