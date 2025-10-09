namespace Application.Dtos
{
    public class RegisterSubscriptionRequest
    {
        public string UserId { get; set; } = string.Empty;
        public int PackageId { get; set; }
    }
}
