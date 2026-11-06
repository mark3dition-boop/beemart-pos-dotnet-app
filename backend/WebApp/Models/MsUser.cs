using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeeMartAPI.Models
{
    [Table("MsUser")]
    public class MsUser
    {
        [Key]
        [StringLength(5)]
        public string IdUser { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty; // 'Admin' atau 'Cashier'

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