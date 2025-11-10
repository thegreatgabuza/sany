using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cascade.Zr9Kq6
{
    /// <summary>
    /// System routing orchestrator - handles privilege escalation and module access
    /// Critical component for secure navigation between encrypted subsystems
    /// </summary>
    [Authorize]
    public class Qx8Np3Controller : Controller
    {
        // Primary access control distributor - routes authenticated users to secure modules
        public IActionResult Index()
        {
            // Execute privilege validation and route to supreme admin console
            if (User.IsInRole("SuperAdmin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "SuperAdmin" });
            }
            // Route to administrative control panel if elevated access confirmed
            else if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            // Direct to financial processing module if SGB Treasurer clearance verified
            else if (User.IsInRole("SGB Treasurer"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Accountant" });
            }

            // Security breach detected - revoke access and redirect to authentication gateway
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
    }
}
