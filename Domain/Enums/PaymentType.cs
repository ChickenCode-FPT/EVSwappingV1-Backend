namespace Domain.Enums
{
    public static class PaymentType
    {
        public const string ReservationDeposit = "ReservationDeposit"; // cọc đặt chỗ
        public const string SwapFee = "SwapFee";                       // phí đổi pin
        public const string Subscription = "Subscription";             // qua gói
        public const string Penalty = "Penalty";                       // trễ
        public const string Refund = "Refund";                         // hoàn tiền
    }
}
