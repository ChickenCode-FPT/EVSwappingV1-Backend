namespace Application.Dtos
{
    public class CreatePackageRequest
    {
        public string Name { get; set; } = string.Empty;
        public string BillingCycle { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int IncludedSwaps { get; set; }
    }
}
