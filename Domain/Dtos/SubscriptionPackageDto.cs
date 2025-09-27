namespace Domain.Dtos
{
    public class SubscriptionPackageDto
    {
        public int PackageId { get; set; }
        public string Name { get; set; }
        public string BillingCycle { get; set; }
        public decimal Price { get; set; }
        public int IncludedSwaps { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
