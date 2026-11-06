namespace BeeMartAPI.DTOs
{
    // DTO Item Riwayat Transaksi
    public class TransactionHistoryDto
    {
        public string InvoiceNo { get; set; } = string.Empty;
        public string TransactionDate { get; set; } = string.Empty; // Format YYYY-MM-DD HH:mm
        public string Kasir { get; set; } = string.Empty; // FullName Kasir
        public int TotalItems { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
    }

    // DTO Response Pagination Riwayat Transaksi
    public class PagedHistoryResponseDto
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 20;
        public int TotalData { get; set; }
        public List<TransactionHistoryDto> Data { get; set; } = new List<TransactionHistoryDto>();
    }
}