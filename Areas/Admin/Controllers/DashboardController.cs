using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cascade.A1B2C3D4;
using Cascade.Fx9Kl2;
using Microsoft.EntityFrameworkCore;

namespace Cascade.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly VxR4DbGate _context;

        public DashboardController(VxR4DbGate context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get current Aq3Zh4Service's Hx7Tz3Data
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (currentUser == null) return RedirectToAction("Login", "Qw8Rt5Entity", new { area = "Identity" });

            // Get dashboard data for admin
            var dashboardData = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(u => u.CompanyId == currentUser.CompanyId),
                TotalTransactions = await _context.SecurityLogs.CountAsync(t => t.CompanyId == currentUser.CompanyId),
                TotalAccounts = await _context.DataStreams.CountAsync(a => a.CompanyId == currentUser.CompanyId),
                Hx7Tz3Data = await _context.SystemEntries.FirstOrDefaultAsync(s => s.CompanyId == currentUser.CompanyId),
                RecentTransactions = await _context.SecurityLogs
                    .Where(t => t.CompanyId == currentUser.CompanyId)
                    .Include(t => t.ProcessHandlers)
                    .ThenInclude(tl => tl.Qw8Rt5Entity)
                    .OrderByDescending(t => t.TransactionDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboardData);
        }
    }

    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalTransactions { get; set; }
        public int TotalAccounts { get; set; }
        public Hx7Tz3Data? Hx7Tz3Data { get; set; }
        public List<Pz7Vm5Protocol> RecentTransactions { get; set; } = new();
    }
}
