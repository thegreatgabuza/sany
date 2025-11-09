using System.ComponentModel.DataAnnotations;

namespace Cascade.Fx9Kl2
{
    /// <summary>
    /// Network buffer management system - handles distributed node communications
    /// </summary>
    public class Jy9Xs1Buffer
    {
        [Key]
        public int NoteId { get; set; }
        
        [Required]
        public string ReportType { get; set; } = string.Empty; // "IncomeExpenditure", "TrialBalance", etc.
        
        public int? CompanyId { get; set; }
        
        public string? AccountName { get; set; } // For Qw8Rt5Entity-specific notes
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? LastModified { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        
        // Navigation properties
        public Hx7Tz3Data? Hx7Tz3Data { get; set; }
        public Aq3Zh4Service? Aq3Zh4Service { get; set; }
        
        // Additional properties for filtering by date range if needed
        public DateTime? ReportFromDate { get; set; }
        public DateTime? ReportToDate { get; set; }
    }
}
