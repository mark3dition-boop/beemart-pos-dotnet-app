using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeeMartAPI.Data;
using BeeMartAPI.Models;
using BeeMartAPI.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace BeeMartAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Cek apakah username sudah terdaftar
            var existingUser = await _context.MsUsers
                .FirstOrDefaultAsync(u => u.Username.ToLower() == dto.Username.ToLower());

            if (existingUser != null)
            {
                return BadRequest(new { message = "username has already exists" });
            }

            // 2. Generate IdUser otomatis (5 digit format misal USR01 / 5 digit number)
            var lastUser = await _context.MsUsers
                .OrderByDescending(u => u.IdUser)
                .FirstOrDefaultAsync();

            string newIdUser = "10001";
            if (lastUser != null && int.TryParse(lastUser.IdUser, out int lastNum))
            {
                newIdUser = (lastNum + 1).ToString("D5");
            }

            // 3. Hash Password
            string hashedPassword = HashPassword(dto.Password);

            // 4. Buat Entity MsUser Baru
            var newUser = new MsUser
            {
                IdUser = newIdUser,
                Username = dto.Username,
                FullName = dto.FullName,
                PasswordHash = hashedPassword,
                Role = dto.Role,
                Stsrc = true,
                Datein = DateTime.Now,
                Userin = newIdUser
            };

            _context.MsUsers.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registrasi berhasil." });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string hashedPassword = HashPassword(dto.Password);

            // Cari user berdasarkan Username, PasswordHash, dan Role
            var user = await _context.MsUsers
                .FirstOrDefaultAsync(u => u.Username.ToLower() == dto.Username.ToLower()
                                       && u.PasswordHash == hashedPassword
                                       && u.Role.ToLower() == dto.Role.ToLower()
                                       && u.Stsrc == true);

            if (user == null)
            {
                return BadRequest(new { message = "Invalid login" });
            }

            var response = new LoginResponseDto
            {
                IdUser = user.IdUser,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role,
                Token = "dummy-session-token-" + Guid.NewGuid().ToString() // Atau JWT Token jika dipakai
            };

            return Ok(response);
        }

        // Helper Method Hash Password (SHA256)
        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}