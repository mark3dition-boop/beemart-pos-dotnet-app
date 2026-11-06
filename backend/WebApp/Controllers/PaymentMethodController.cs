using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeeMartAPI.Data;
using BeeMartAPI.DTOs;

namespace BeeMartAPI.Controllers
{
    [ApiController]
    [Route("api/payment-methods")]
    public class PaymentMethodController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentMethodController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/payment-methods
        [HttpGet]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var methods = await _context.LtPaymentMethods
                .Where(p => p.Stsrc == true)
                .Select(p => new PaymentMethodResponseDto
                {
                    IdPaymentMethod = p.IdPaymentMethod,
                    MethodName = p.MethodName
                })
                .ToListAsync();

            return Ok(methods);
        }
    }
}