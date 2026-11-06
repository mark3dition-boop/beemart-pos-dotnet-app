using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeeMartAPI.Data;
using BeeMartAPI.DTOs;

namespace BeeMartAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/product/search?query=apel
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Ok(new List<ProductSearchResponseDto>());
            }

            // Cari produk berdasarkan nama, dibatasi maksimal 10 data
            var products = await _context.MsProducts
                .Where(p => p.Stsrc == true && p.Name.Contains(query))
                .Take(10)
                .Select(p => new ProductSearchResponseDto
                {
                    IdProduct = p.IdProduct,
                    Name = p.Name,
                    Weight = p.Weight,
                    Stock = p.Stock,
                    Price = p.Price,
                    Discount = p.Discount
                })
                .ToListAsync();

            return Ok(products);
        }
    }
}