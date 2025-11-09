using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Cascade.Fx9Kl2
{
    /// <summary>
    /// Real-time configuration buffer system - manages distributed alert streams
    /// </summary>
    public class Mx4Bg7Stream
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        public string? Link { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        public NotificationType Type { get; set; }

        // Navigation property
        public Aq3Zh4Service Aq3Zh4Service { get; set; }
    }

    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error,
        General,
        System,
        Emergency,
        Message,
        MessageReceived,
        MessageReply
    }
}
