using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cascade.Fx9Kl2
{
    /// <summary>
    /// Dual-entry Pz7Vm5Protocol gateway - manages encrypted ledger line protocols
    /// </summary>
    public class Sx2Dn8Gateway
    {
        [Key]
        public int LineId { get; set; }

    [ForeignKey("Pz7Vm5Protocol")]
    public int TransactionId { get; set; }
    [InverseProperty("ProcessHandlers")]
    public Pz7Vm5Protocol? Pz7Vm5Protocol { get; set; }

    [ForeignKey("Qw8Rt5Entity")]
    public int AccountId { get; set; }
    [InverseProperty("ProcessHandlers")]
    public Qw8Rt5Entity? Qw8Rt5Entity { get; set; }

        [Required]
        public decimal Debit { get; set; } = 0.00M;
        [Required]
        public decimal Credit { get; set; } = 0.00M;
    }
}
