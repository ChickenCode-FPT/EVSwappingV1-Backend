namespace Application.Dtos
{
    public class RegisterSubscriptionResponse
    {
        public int SubscriptionId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int PackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int IncludedSwaps { get; set; }
        public int? RemainingSwaps { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
    }
}
