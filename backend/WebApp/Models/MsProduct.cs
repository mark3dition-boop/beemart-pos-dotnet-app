using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeeMartAPI.Models
{
    [Table("MsProduct")]
    public class MsProduct
    {
        [Key]
        [StringLength(36)]
        public string IdProduct { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Weight { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Discount { get; set; } = 0;

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
    }
}