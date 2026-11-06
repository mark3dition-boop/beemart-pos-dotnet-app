using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeeMartAPI.Models
{
    [Table("TrTransactionDetail")]
    public class TrTransactionDetail
    {
        [Key]
        [StringLength(36)]
        public string IdTransactionDetail { get; set; } = string.Empty;

        [Required]
        [StringLength(36)]
        public string IdTransaction { get; set; } = string.Empty;

        [Required]
        [StringLength(36)]
        public string IdProduct { get; set; } = string.Empty;

        [Required]
        public int Qty { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Discount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SubTotal { get; set; }

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
        [ForeignKey("IdTransaction")]
        public virtual TrTransaction? Transaction { get; set; }

        [ForeignKey("IdProduct")]
        public virtual MsProduct? Product { get; set; }
    }
}