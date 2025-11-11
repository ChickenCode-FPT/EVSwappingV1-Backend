namespace Application.Dtos.Subscription
{
    public class RegisterSubscriptionRequest
    {
        public string UserId { get; set; } = string.Empty;
        public int PackageId { get; set; }
    }
}
