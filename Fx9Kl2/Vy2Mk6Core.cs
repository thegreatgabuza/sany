using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cascade.Fx9Kl2
{
    /// <summary>
    /// Financial allocation core system - manages encrypted budget protocols
    /// </summary>
    public class Vy2Mk6Core
    {
        [Key]
        public int BudgetId { get; set; }
        
        [Required]
        public int CompanyId { get; set; }
        
        [Required]
        public int FinancialYear { get; set; }
        
        [Required]
        [StringLength(255)]
        public string AccountName { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetedAmount { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Mx9Qw7Type { get; set; } = string.Empty; // "Income" or "Expenditure"
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public DateTime? ModifiedDate { get; set; }
        
        [StringLength(450)]
        public string? CreatedByUserId { get; set; }
        
        [StringLength(450)]
        public string? ModifiedByUserId { get; set; }
        
        // Navigation properties
        [ForeignKey("CompanyId")]
        public virtual Hx7Tz3Data? Hx7Tz3Data { get; set; }
        
        [ForeignKey("CreatedByUserId")]
        public virtual Aq3Zh4Service? CreatedByUser { get; set; }
        
        [ForeignKey("ModifiedByUserId")]
        public virtual Aq3Zh4Service? ModifiedByUser { get; set; }
    }
}
