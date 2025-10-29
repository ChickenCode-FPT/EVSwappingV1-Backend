namespace Domain.Enums
{
    public enum PaymentStatus
    {
        Pending = 0,
        Paid = 1,
        Failed = 2
    }

    public static class PaymentStatus2
    {
        public const string Pending = "Pending";       // Chưa thanh toán
        public const string Paid = "Paid";             // Đã thanh toán thành công
        public const string Refunded = "Refunded";     // Đã hoàn tiền
        public const string Forfeit = "Forfeit";       // Bị giữ lại / mất cọc
        public const string Cancelled = "Cancelled";   // Đơn bị hủy (user hoặc lỗi)
    }
}
