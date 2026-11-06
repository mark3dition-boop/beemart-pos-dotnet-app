using System.ComponentModel.DataAnnotations;

namespace BeeMartAPI.DTOs
{
    // DTO Request Item Keranjang
    public class CreateTransactionDetailDto
    {
        [Required]
        public string IdProduct { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Qty minimal 1.")]
        public int Qty { get; set; }
    }

    // DTO Request Submit Transaksi Baru
    public class CreateTransactionRequestDto
    {
        [Required]
        public string IdUser { get; set; } = string.Empty;

        [Required]
        public int IdPaymentMethod { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Keranjang belanja tidak boleh kosong.")]
        public List<CreateTransactionDetailDto> Items { get; set; } = new List<CreateTransactionDetailDto>();
    }

    // DTO Response Submit Transaksi
    public class CreateTransactionResponseDto
    {
        public string IdTransaction { get; set; } = string.Empty;
        public string InvoiceNo { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}