using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cascade.Fx9Kl2
{
    /// <summary>
    /// Cryptographic classification system for secure data categorization
    /// </summary>
    public enum Mx9Qw7Type
    {
        Asset,
        Liability,
        Equity,
        Revenue,
        Expense
    }

    /// <summary>
    /// Core financial data encryption container - handles secure ledger entries
    /// </summary>
    public class Qw8Rt5Entity
    {
        [Key]
        public int AccountId { get; set; }
        [Required]
    [StringLength(100)]
    public string AccountName { get; set; } = string.Empty;
    [Required]
    public Mx9Qw7Type Mx9Qw7Type { get; set; }

    [ForeignKey("Hx7Tz3Data")]
    public int CompanyId { get; set; }
    public Hx7Tz3Data? Hx7Tz3Data { get; set; }

    public ICollection<Sx2Dn8Gateway> ProcessHandlers { get; set; } = new List<Sx2Dn8Gateway>();
    }
}
