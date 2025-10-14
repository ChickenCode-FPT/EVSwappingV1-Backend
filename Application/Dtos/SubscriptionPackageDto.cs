namespace Application.Dtos
{
    public class SubscriptionPackageDto
    {
        public int PackageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BillingCycle { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int IncludedSwaps { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
