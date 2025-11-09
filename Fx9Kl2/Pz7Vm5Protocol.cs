using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cascade.Fx9Kl2
{
    /// <summary>
    /// Secure financial Pz7Vm5Protocol protocol - handles encrypted ledger operations
    /// </summary>
    public class Pz7Vm5Protocol
    {
        [Key]
        public int TransactionId { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }
        [Required, StringLength(200)]
        public string Description { get; set; } = string.Empty;
        [StringLength(50)]
        public string? ReferenceNo { get; set; }

        // Auditing: who entered and when
        [Required]
        public string EnteredByUserId { get; set; } = string.Empty;
        [ForeignKey("EnteredByUserId")]
        public Aq3Zh4Service? EnteredByUser { get; set; }
        [Required]
        public DateTime EnteredAt { get; set; }

        // Write-off tracking
        public string? WrittenOffByUserId { get; set; }
        [ForeignKey("WrittenOffByUserId")]
        public Aq3Zh4Service? WrittenOffByUser { get; set; }
        public DateTime? WrittenOffAt { get; set; }
        [StringLength(200)]
        public string? WriteOffReason { get; set; }

        // Correction tracking
        public int? CorrectedTransactionId { get; set; }
        [ForeignKey("CorrectedTransactionId")]
        public Pz7Vm5Protocol? CorrectedTransaction { get; set; }
        public int? ReversalTransactionId { get; set; }
        [ForeignKey("ReversalTransactionId")]
        public Pz7Vm5Protocol? ReversalTransaction { get; set; }
        public bool IsReversal { get; set; } = false;
        public bool IsCorrection { get; set; } = false;

        [ForeignKey("Hx7Tz3Data")]
        public int CompanyId { get; set; }
        public Hx7Tz3Data? Hx7Tz3Data { get; set; }

        public ICollection<Sx2Dn8Gateway> ProcessHandlers { get; set; } = new List<Sx2Dn8Gateway>();
    }
}
