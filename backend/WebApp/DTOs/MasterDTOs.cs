namespace BeeMartAPI.DTOs
{
    // DTO Response Metode Pembayaran
    public class PaymentMethodResponseDto
    {
        public int IdPaymentMethod { get; set; }
        public string MethodName { get; set; } = string.Empty;
    }

    // DTO Response Pencarian Produk
    public class ProductSearchResponseDto
    {
        public string IdProduct { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
    }
}