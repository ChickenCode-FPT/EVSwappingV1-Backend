namespace Application.Dtos.Subscription
{
    public class SubscriptionDto
    {
        public int SubscriptionId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string BillingCycle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? RemainingSwaps { get; set; }
    }

    public class SubscriptionPaymentDto
    {
        public string UserId { get; set; } = string.Empty;
        public int PackageId { get; set; }
        public string BillingCycle { get; set; } = Domain.Enums.BillingCycle.Monthly;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public string Method { get; set; } = "PayOS";
        public string Description { get; set; } = string.Empty;
        public string? TransactionRef { get; set; }
    }
}
