namespace Application.Dtos
{
    public class RegisterDriverResponse
    {
        public string UserId { get; set; } = string.Empty;
        public int DriverId { get; set; }
        public string PreferredPaymentMethod { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
