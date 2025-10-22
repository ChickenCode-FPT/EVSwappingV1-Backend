namespace Application.Dtos
{
    public class PaymentUpdateDto
    {
        public long PaymentId { get; set; }

        public long? SwapTransactionId { get; set; }

        public string UserId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string Method { get; set; }

        public string Status { get; set; }

        public string TransactionRef { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
