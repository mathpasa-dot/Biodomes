// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using BioDomes.Infrastructures.Model;
using BioDomes.Web.ValidationRules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BioDomes.Web.Pages.Identity;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IWebHostEnvironment _env;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public IndexModel(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IWebHostEnvironment env)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _env = env;
    }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }
        
        // Username
        var userName = await _userManager.GetUserNameAsync(user);
        if (Input.Username != userName)
        {
            var setNameResult = await _userManager.SetUserNameAsync(user, Input.Username);
            if (!setNameResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to set userName.";
                return RedirectToPage();
            }
        }

        // Phone
        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (Input.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to set phone number.";
                return RedirectToPage();
            }
        }

        // DateOfBirth
        if (Input.DateOfBirth.HasValue && Input.DateOfBirth.Value != user.DateOfBirth)
            user.DateOfBirth = Input.DateOfBirth.Value;

        // ResearchTeam
        if (Input.ResearchTeam != user.ResearchTeam)
            user.ResearchTeam = Input.ResearchTeam;

        // Avatar
        var savedPath = await SaveImage();
        if (savedPath != null)
            user.UserAvatar = savedPath;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            StatusMessage = "Unexpected error when trying to update profile.";
            return RedirectToPage();
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }
    
    private async Task LoadAsync(User user)
    {
        var userName = await _userManager.GetUserNameAsync(user);
        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        Input = new InputModel
        {
            Username = userName,
            PhoneNumber = phoneNumber,
            DateOfBirth = user.DateOfBirth,
            ResearchTeam = user.ResearchTeam,
            CurrentAvatarPath = user.UserAvatar ?? "/images/avatars/default.jpeg"
        };
    }

    protected async Task<string?> SaveImage()
    {
        if (Input.UserAvatarFile == null || Input.UserAvatarFile.Length == 0) return null;
        
        var user = await _userManager.GetUserAsync(User);
        var currentAvatarPath = user!.UserAvatar;

        if (!string.IsNullOrEmpty(currentAvatarPath))
        {
            var existingFilePath = Path.Combine(_env.WebRootPath, currentAvatarPath.TrimStart('/'));
            if (System.IO.File.Exists(existingFilePath)) System.IO.File.Delete(existingFilePath);
        }
        
        var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "avatars");
        Directory.CreateDirectory(uploadsFolder);
        
        var email = await _userManager.GetEmailAsync(user!);

        var extension = Path.GetExtension(Input.UserAvatarFile.FileName);
        var fileName = $"{email}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await Input.UserAvatarFile.CopyToAsync(stream);

        return $"/images/avatars/{fileName}";
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Ce champ est requis.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ\s\-]+$", ErrorMessage = "Le nom ne peut contenir que des lettres et des espaces.")]
        [Display(Name = "Nom")]
        public string Username { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Téléphone")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date de naissance")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Equipe de recherche")] 
        public string? ResearchTeam { get; set; } = string.Empty;

        [ValidFile]
        public IFormFile? UserAvatarFile { get; set; }
        
        public string? CurrentAvatarPath { get; set; }
    }
}