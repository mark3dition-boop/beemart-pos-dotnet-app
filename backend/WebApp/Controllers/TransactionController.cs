using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeeMartAPI.Data;
using BeeMartAPI.Models;
using BeeMartAPI.DTOs;

namespace BeeMartAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/transaction
        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Validasi User
                var user = await _context.MsUsers.FindAsync(dto.IdUser);
                if (user == null)
                    return BadRequest(new { message = "User tidak ditemukan." });

                // 2. Generate Invoice Number: [5 digit userID][ddmmyy][3 digit running number]
                string dateStr = DateTime.Now.ToString("ddMMyy");
                string invoicePrefix = $"{user.IdUser.PadLeft(5, '0')}{dateStr}";

                var todayTransactionsCount = await _context.TrTransactions
                    .Where(t => t.InvoiceNo.StartsWith(invoicePrefix))
                    .CountAsync();

                string runningNumber = (todayTransactionsCount + 1).ToString("D3");
                string invoiceNo = $"{invoicePrefix}{runningNumber}";

                // 3. Proses Items & Kurangi Stok
                decimal totalPrice = 0;
                string transactionId = Guid.NewGuid().ToString();
                var transactionDetails = new List<TrTransactionDetail>();

                foreach (var item in dto.Items)
                {
                    var product = await _context.MsProducts.FindAsync(item.IdProduct);
                    if (product == null)
                        return BadRequest(new { message = $"Produk dengan ID {item.IdProduct} tidak ditemukan." });

                    // Validasi Stok
                    if (item.Qty > product.Stock)
                    {
                        return BadRequest(new { message = "Jumlah barang yang dimasukkan melebihi sisa stok." });
                    }

                    // Potong Stok
                    product.Stock -= item.Qty;
                    product.Dateup = DateTime.Now;
                    product.Userup = dto.IdUser;

                    // Hitung Subtotal
                    decimal discountAmount = product.Price * (product.Discount / 100);
                    decimal finalPrice = product.Price - discountAmount;
                    decimal subTotal = finalPrice * item.Qty;
                    totalPrice += subTotal;

                    // Buat Detail Entity
                    transactionDetails.Add(new TrTransactionDetail
                    {
                        IdTransactionDetail = Guid.NewGuid().ToString(),
                        IdTransaction = transactionId,
                        IdProduct = item.IdProduct,
                        Qty = item.Qty,
                        Price = product.Price,
                        Discount = product.Discount,
                        SubTotal = subTotal,
                        Stsrc = true,
                        Datein = DateTime.Now,
                        Userin = dto.IdUser
                    });
                }

                // 4. Buat Header Transaction Entity
                var transactionHeader = new TrTransaction
                {
                    IdTransaction = transactionId,
                    InvoiceNo = invoiceNo,
                    TransactionDate = DateTime.Now,
                    IdUser = dto.IdUser,
                    IdPaymentMethod = dto.IdPaymentMethod,
                    TotalPrice = totalPrice,
                    Stsrc = true,
                    Datein = DateTime.Now,
                    Userin = dto.IdUser,
                    TransactionDetails = transactionDetails
                };

                _context.TrTransactions.Add(transactionHeader);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new CreateTransactionResponseDto
                {
                    IdTransaction = transactionId,
                    InvoiceNo = invoiceNo,
                    TotalPrice = totalPrice,
                    Message = "Transaksi berhasil diproses."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Terjadi kesalahan internal: " + ex.Message });
            }
        }

        // GET: api/transaction/history?userId=10001&role=Cashier&date=2026-09-04&page=1
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(
            [FromQuery] string userId,
            [FromQuery] string role,
            [FromQuery] DateTime date,
            [FromQuery] int page = 1)
        {
            int pageSize = 20;

            var query = _context.TrTransactions
                .Include(t => t.User)
                .Include(t => t.PaymentMethod)
                .Include(t => t.TransactionDetails)
                .Where(t => t.Stsrc == true && t.TransactionDate.Date == date.Date);

            // Filter Role Access Control
            if (role.Equals("Cashier", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(t => t.IdUser == userId);
            }

            // Order By Ascending berdasarkan Tanggal & Jam Transaksi
            query = query.OrderBy(t => t.TransactionDate);

            int totalData = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalData / pageSize);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionHistoryDto
                {
                    InvoiceNo = t.InvoiceNo,
                    TransactionDate = t.TransactionDate.ToString("yyyy-MM-dd HH:mm"),
                    Kasir = t.User != null ? t.User.FullName : string.Empty,
                    TotalItems = t.TransactionDetails.Sum(d => d.Qty),
                    PaymentMethod = t.PaymentMethod != null ? t.PaymentMethod.MethodName : string.Empty,
                    TotalPrice = t.TotalPrice
                })
                .ToListAsync();

            var response = new PagedHistoryResponseDto
            {
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalData = totalData,
                Data = items
            };

            return Ok(response);
        }
    }
}