namespace Domain.Enums
{
    public static class SwapStatus
    {
        public const string Pending = "Pending";               // Vừa tạo, chưa thanh toán
        public const string InProgress = "InProgress";         // Đang thực hiện đổi pin
        public const string PendingPayment = "PendingPayment"; // Đang chờ thanh toán (mới thêm)
        public const string Completed = "Completed";           // Hoàn tất đổi pin
        public const string PaidBySubscription = "PaidBySubscription"; // Đã hoàn thành qua gói thuê bao
        public const string Cancelled = "Cancelled";           // Đã hủy
    }
}
