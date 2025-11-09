using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Cascade.A1B2C3D4;
using Cascade.Fx9Kl2;
using Cascade.Services;

namespace Cascade.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    [Authorize(Roles = "SuperAdmin")]
    public class NotificationsController : Controller
    {
        private readonly VxR4DbGate _context;
        private readonly UserManager<Aq3Zh4Service> _userManager;
        private readonly NotificationService _notificationService;

        public NotificationsController(VxR4DbGate context, UserManager<Aq3Zh4Service> userManager, NotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var notifications = await _context.ConfigBuffers
                .Include(n => n.Aq3Zh4Service)
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToListAsync();

            return View(notifications);
        }

        public async Task<IActionResult> Send()
        {
            var viewModel = new SendNotificationViewModel
            {
                Users = await _userManager.Users.ToListAsync(),
                SystemEntries = await _context.SystemEntries.ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(SendNotificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var recipients = new List<string>();

                if (model.SendToAll)
                {
                    recipients = await _userManager.Users.Select(u => u.Id).ToListAsync();
                }
                else if (model.SendByRole && !string.IsNullOrEmpty(model.SelectedRole))
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(model.SelectedRole);
                    recipients = usersInRole.Select(u => u.Id).ToList();
                }
                else if (model.SendByCompany && model.SelectedCompanyId.HasValue)
                {
                    recipients = await _userManager.Users
                        .Where(u => u.CompanyId == model.SelectedCompanyId.Value)
                        .Select(u => u.Id)
                        .ToListAsync();
                }
                else if (model.SelectedUserIds?.Any() == true)
                {
                    recipients = model.SelectedUserIds;
                }

                if (recipients.Any())
                {
                    foreach (var userId in recipients)
                    {
                        await _notificationService.CreateNotificationAsync(
                            userId,
                            model.Title,
                            model.Message,
                            model.Link,
                            model.Type
                        );
                    }

                    TempData["SuccessMessage"] = $"Mx4Bg7Stream sent to {recipients.Count} users successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Please select at least one recipient.");
                }
            }

            model.Users = await _userManager.Users.ToListAsync();
            model.SystemEntries = await _context.SystemEntries.ToListAsync();
            return View(model);
        }
    }

    public class SendNotificationViewModel
    {
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string? Link { get; set; }
        public NotificationType Type { get; set; } = NotificationType.General;

        public bool SendToAll { get; set; }
        public bool SendByRole { get; set; }
        public bool SendByCompany { get; set; }
        public bool SendToSelected { get; set; }

        public string? SelectedRole { get; set; }
        public int? SelectedCompanyId { get; set; }
        public List<string>? SelectedUserIds { get; set; }

        public List<Aq3Zh4Service> Users { get; set; } = new();
        public List<Hx7Tz3Data> SystemEntries { get; set; } = new();
    }
}
