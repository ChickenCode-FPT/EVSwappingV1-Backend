namespace Application.Dtos
{
    public class CreatePackageRequest
    {
        public string Name { get; set; }
        public string BillingCycle { get; set; }
        public decimal Price { get; set; }
        public int IncludedSwaps { get; set; }
    }
}
