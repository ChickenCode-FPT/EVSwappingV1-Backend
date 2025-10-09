namespace Application.Dtos
{
    public class RegisterDriverRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string PreferredPaymentMethod { get; set; } = string.Empty;
    }
}
