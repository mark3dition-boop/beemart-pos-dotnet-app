using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeeMartAPI.Models
{
    [Table("LtPaymentMethod")]
    public class LtPaymentMethod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPaymentMethod { get; set; }

        [Required]
        [StringLength(50)]
        public string MethodName { get; set; } = string.Empty;

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