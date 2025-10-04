namespace Application.Dtos
{
    public class RegisterSubscriptionResponse
    {
        public int SubscriptionId { get; set; }
        public string UserId { get; set; }
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public decimal Price { get; set; }
        public int IncludedSwaps { get; set; }
        public int? RemainingSwaps { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
    }
}
