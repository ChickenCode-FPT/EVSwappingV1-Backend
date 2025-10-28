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
        public const string Pending = "Pending";
        public const string Paid = "Paid";
        public const string Refunded = "Refunded";
        public const string Forfeit = "Forfeit";       // mất cọc
        public const string Cancelled = "Cancelled";
    }
}
