using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeeMartAPI.Models
{
    [Table("TrTransaction")]
    public class TrTransaction
    {
        [Key]
        [StringLength(36)]
        public string IdTransaction { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string InvoiceNo { get; set; } = string.Empty;

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        [StringLength(5)]
        public string IdUser { get; set; } = string.Empty;

        [Required]
        public int IdPaymentMethod { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        public bool Stsrc { get; set; }

        [Required]
        public DateTime Datein { get; set; }

        [Required]
        [StringLength(36)]
        public string Userin { get; set; } = string.Empty;

        public DateTime? Dateup { get; set; }

        [StringLength(36)]
        public string? Userup { get; set; }

        // Navigation Properties
        [ForeignKey("IdUser")]
        public virtual MsUser? User { get; set; }

        [ForeignKey("IdPaymentMethod")]
        public virtual LtPaymentMethod? PaymentMethod { get; set; }

        public virtual ICollection<TrTransactionDetail> TransactionDetails { get; set; } = new List<TrTransactionDetail>();
    }
}