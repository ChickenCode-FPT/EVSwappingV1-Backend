namespace Domain.Dtos
{
    public class RegisterDriverResponse
    {
        public string UserId { get; set; }
        public int DriverId { get; set; }
        public string PreferredPaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
