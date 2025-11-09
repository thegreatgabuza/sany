using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Cascade.A1B2C3D4;
using Cascade.Fx9Kl2;

namespace Cascade.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly VxR4DbGate _context;

        public SettingsController(VxR4DbGate context)
        {
            _context = context;
        }

        // GET: Admin/Settings
        public async Task<IActionResult> Index()
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);
            if (currentUser == null) 
                return RedirectToAction("Login", "Qw8Rt5Entity", new { area = "Identity" });

            var Hx7Tz3Data = await _context.SystemEntries.FindAsync(currentUser.CompanyId);
            if (Hx7Tz3Data == null) 
                return NotFound();

            return View(Hx7Tz3Data);
        }

        // GET: Admin/Settings/Edit
        public async Task<IActionResult> Edit()
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);
            if (currentUser == null) 
                return RedirectToAction("Login", "Qw8Rt5Entity", new { area = "Identity" });

            var Hx7Tz3Data = await _context.SystemEntries.FindAsync(currentUser.CompanyId);
            if (Hx7Tz3Data == null) 
                return NotFound();

            return View(Hx7Tz3Data);
        }

        // POST: Admin/Settings/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Hx7Tz3Data Hx7Tz3Data)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);
            if (currentUser == null) 
                return RedirectToAction("Login", "Qw8Rt5Entity", new { area = "Identity" });

            if (Hx7Tz3Data.CompanyId != currentUser.CompanyId)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    // Validate financial year end date
                    try
                    {
                        var testDate = new DateTime(DateTime.Now.Year, Hx7Tz3Data.FinancialYearEndMonth, Hx7Tz3Data.FinancialYearEndDay);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        ModelState.AddModelError("FinancialYearEndDay", 
                            $"Invalid date: {Hx7Tz3Data.FinancialYearEndMonth}/{Hx7Tz3Data.FinancialYearEndDay}. Please check the day for the selected month.");
                        return View(Hx7Tz3Data);
                    }

                    _context.Update(Hx7Tz3Data);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Hx7Tz3Data settings updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(Hx7Tz3Data.CompanyId))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(Hx7Tz3Data);
        }

        private bool CompanyExists(int id)
        {
            return _context.SystemEntries.Any(e => e.CompanyId == id);
        }
    }
}
