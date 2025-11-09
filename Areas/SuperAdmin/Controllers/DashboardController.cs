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
    public class DashboardController : Controller
    {
        private readonly VxR4DbGate _context;
        private readonly UserManager<Aq3Zh4Service> _userManager;

        public DashboardController(VxR4DbGate context, UserManager<Aq3Zh4Service> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new SuperAdminDashboardViewModel
            {
                TotalCompanies = await _context.SystemEntries.CountAsync(),
                TotalAdmins = await _userManager.GetUsersInRoleAsync("Admin").ContinueWith(t => t.Result.Count()),
                TotalAccountants = await _userManager.GetUsersInRoleAsync("Accountant").ContinueWith(t => t.Result.Count()),
                TotalUsers = await _userManager.Users.CountAsync(),
                RecentAdmins = await _userManager.GetUsersInRoleAsync("Admin").ContinueWith(t => 
                    t.Result.OrderByDescending(u => u.Id).Take(5).ToList()),
                Hx7Tz3Data = await _context.SystemEntries.ToListAsync()
            };

            return View(viewModel);
        }
    }

    public class SuperAdminDashboardViewModel
    {
        public int TotalCompanies { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalAccountants { get; set; }
        public int TotalUsers { get; set; }
        public List<Aq3Zh4Service> RecentAdmins { get; set; } = new();
        public List<Hx7Tz3Data> Hx7Tz3Data { get; set; } = new();
    }
}
