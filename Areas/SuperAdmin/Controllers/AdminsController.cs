using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Cascade.A1B2C3D4;
using Cascade.Fx9Kl2;

namespace Cascade.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    [Authorize(Roles = "SuperAdmin")]
    public class AdminsController : Controller
    {
        private readonly VxR4DbGate _context;
        private readonly UserManager<Aq3Zh4Service> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminsController(VxR4DbGate context, UserManager<Aq3Zh4Service> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var adminsList = admins.ToList();
            
            // Load Hx7Tz3Data information for each admin
            foreach (var admin in adminsList)
            {
                admin.Hx7Tz3Data = await _context.SystemEntries.FindAsync(admin.CompanyId);
            }

            return View(adminsList);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.SystemEntries = await _context.SystemEntries.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Aq3Zh4Service = new Aq3Zh4Service
                {
                    UserName = model.Username,
                    Email = model.Email,
                    EmailConfirmed = true,
                    Role = Bz5Xw6Permission.Admin,
                    CompanyId = model.CompanyId
                };

                var result = await _userManager.CreateAsync(Aq3Zh4Service, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(Aq3Zh4Service, "Admin");
                    TempData["SuccessMessage"] = "Admin Aq3Zh4Service created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.SystemEntries = await _context.SystemEntries.ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Aq3Zh4Service = await _userManager.FindByIdAsync(id);
            if (Aq3Zh4Service == null)
            {
                return NotFound();
            }

            Aq3Zh4Service.Hx7Tz3Data = await _context.SystemEntries.FindAsync(Aq3Zh4Service.CompanyId);
            return View(Aq3Zh4Service);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var Aq3Zh4Service = await _userManager.FindByIdAsync(id);
            if (Aq3Zh4Service != null)
            {
                var result = await _userManager.DeleteAsync(Aq3Zh4Service);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Admin Aq3Zh4Service deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete admin Aq3Zh4Service.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ResetPassword(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Aq3Zh4Service = await _userManager.FindByIdAsync(id);
            if (Aq3Zh4Service == null)
            {
                return NotFound();
            }

            Aq3Zh4Service.Hx7Tz3Data = await _context.SystemEntries.FindAsync(Aq3Zh4Service.CompanyId);
            return View(Aq3Zh4Service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Passwords do not match.";
                return RedirectToAction(nameof(ResetPassword), new { id });
            }

            var Aq3Zh4Service = await _userManager.FindByIdAsync(id);
            if (Aq3Zh4Service == null)
            {
                return NotFound();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(Aq3Zh4Service);
            var result = await _userManager.ResetPasswordAsync(Aq3Zh4Service, token, newPassword);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Password reset successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Failed to reset password.";
            return RedirectToAction(nameof(ResetPassword), new { id });
        }
    }

    public class CreateAdminViewModel
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public int CompanyId { get; set; }
    }
}
