namespace Domain.Enums
{
    public static class SwapStatus
    {
        public const string Pending = "Pending";
        public const string InProgress = "InProgress";         // d9ang thực hiện đổi pin
        public const string PendingPayment = "PendingPayment"; // đang chờ tt
        public const string Completed = "Completed";           // hoàn tất đổi pin
        public const string PaidBySubscription = "PaidBySubscription"; // đả hoàn thành qua gói
        public const string Cancelled = "Cancelled";
    }
}
