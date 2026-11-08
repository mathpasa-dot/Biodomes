// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel.DataAnnotations;
using BioDomes.Infrastructures.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BioDomes.Web.Pages.Identity;

[Authorize]
public class ChangePasswordModel : PageModel
{
    private readonly ILogger<ChangePasswordModel> _logger;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public ChangePasswordModel(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILogger<ChangePasswordModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }
    
    [BindProperty]
    public InputModel Input { get; set; }
    
    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (!hasPassword) return RedirectToPage("./SetPassword");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            foreach (var error in changePasswordResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        await _signInManager.RefreshSignInAsync(user);
        _logger.LogInformation("User changed their password successfully.");
        StatusMessage = "Your password has been changed.";

        return RedirectToPage();
    }
    
    public class InputModel
    {
        [Required(ErrorMessage = "Ce champ est requis.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Le mot de passe doit être entre 6 et 100 caractères.",
            MinimumLength = 6)]
        [Display(Name = "Ancien MDP")]
        public string OldPassword { get; set; }
        
        [Required(ErrorMessage = "Ce champ est requis.")]
        [StringLength(100, ErrorMessage = "Le mot de passe doit être entre 6 et 100 caractères.",
                    MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau MDP")]
        public string NewPassword { get; set; }
        
        [Required(ErrorMessage = "Ce champ est requis.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "La confirmation ne correspond pas au mot de passe.")]
        [Display(Name = "Confirmer nouveau MDP")]
        public string ConfirmPassword { get; set; }
    }
}