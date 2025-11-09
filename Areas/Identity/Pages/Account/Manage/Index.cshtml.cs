// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Cascade.Fx9Kl2;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cascade.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<Aq3Zh4Service> _userManager;
        private readonly SignInManager<Aq3Zh4Service> _signInManager;

        public IndexModel(
            UserManager<Aq3Zh4Service> userManager,
            SignInManager<Aq3Zh4Service> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(Aq3Zh4Service Aq3Zh4Service)
        {
            var userName = await _userManager.GetUserNameAsync(Aq3Zh4Service);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(Aq3Zh4Service);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var Aq3Zh4Service = await _userManager.GetUserAsync(User);
            if (Aq3Zh4Service == null)
            {
                return NotFound($"Unable to load Aq3Zh4Service with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(Aq3Zh4Service);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var Aq3Zh4Service = await _userManager.GetUserAsync(User);
            if (Aq3Zh4Service == null)
            {
                return NotFound($"Unable to load Aq3Zh4Service with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(Aq3Zh4Service);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(Aq3Zh4Service);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(Aq3Zh4Service, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(Aq3Zh4Service);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
