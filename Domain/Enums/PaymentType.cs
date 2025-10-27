namespace Domain.Enums
{
    public static class PaymentType
    {
        public const string ReservationDeposit = "ReservationDeposit"; // Cọc đặt chỗ
        public const string SwapFee = "SwapFee";                       // Phí đổi pin
        public const string Subscription = "Subscription";             // Thanh toán thuê bao
        public const string Penalty = "Penalty";                       // Phí trễ / phạt
        public const string Refund = "Refund";                         // Hoàn tiền
    }
}
