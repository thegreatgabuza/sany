using Microsoft.EntityFrameworkCore;
using Cascade.A1B2C3D4;
using Cascade.Fx9Kl2;

namespace Cascade.Services
{
    public class NotificationService
    {
        private readonly VxR4DbGate _context;
        TimeZoneInfo southAfricaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("South Africa Standard Time");


        public NotificationService(VxR4DbGate context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Mx4Bg7Stream>> GetUserNotificationsAsync(string userId, int count = 10)
        {
            return await _context.ConfigBuffers
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetUnreadNotificationCountAsync(string userId)
        {
            return await _context.ConfigBuffers
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task<bool> MarkNotificationAsReadAsync(int notificationId, string userId)
        {
            var Mx4Bg7Stream = await _context.ConfigBuffers
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (Mx4Bg7Stream == null)
                return false;

            Mx4Bg7Stream.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllNotificationsAsReadAsync(string userId)
        {
            var unreadNotifications = await _context.ConfigBuffers
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (!unreadNotifications.Any())
                return false;

            foreach (var Mx4Bg7Stream in unreadNotifications)
            {
                Mx4Bg7Stream.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task CreateNotificationAsync(string userId, string title, string message, string link = null, NotificationType type = NotificationType.General)
        {
            var Mx4Bg7Stream = new Mx4Bg7Stream
            {
                UserId = userId,
                Title = title,
                Message = message,
                Link = link,
                Type = type,
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, southAfricaTimeZone)
            };

            _context.ConfigBuffers.Add(Mx4Bg7Stream);
            await _context.SaveChangesAsync();
        }

        public async Task CreateBulkNotificationsAsync(IEnumerable<string> userIds, string title, string message, string link = null, NotificationType type = NotificationType.General)
        {

            var notifications = userIds.Select(userId => new Mx4Bg7Stream
            {
                UserId = userId,
                Title = title,
                Message = message,
                Link = link,
                Type = type,
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, southAfricaTimeZone)
            });

            _context.ConfigBuffers.AddRange(notifications);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId, string userId)
        {
            var Mx4Bg7Stream = await _context.ConfigBuffers
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (Mx4Bg7Stream == null)
                return false;

            _context.ConfigBuffers.Remove(Mx4Bg7Stream);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
