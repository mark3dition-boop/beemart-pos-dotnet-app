using System.ComponentModel.DataAnnotations;

namespace BeeMartAPI.DTOs
{
    // DTO untuk Request Register
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Username wajib diisi.")]
        [StringLength(50, ErrorMessage = "Username maksimal 50 karakter.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Name wajib diisi.")]
        [StringLength(300, ErrorMessage = "Full Name maksimal 300 karakter.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Re-Password wajib diisi.")]
        [Compare("Password", ErrorMessage = "Password dan Re-Password tidak cocok.")]
        public string RePassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role wajib diisi.")]
        public string Role { get; set; } = string.Empty; // "Admin" atau "Cashier"
    }

    // DTO untuk Request Login
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username wajib diisi.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role wajib diisi.")]
        public string Role { get; set; } = string.Empty; // "Admin" atau "Cashier"
    }

    // DTO untuk Response Login
    public class LoginResponseDto
    {
        public string IdUser { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}