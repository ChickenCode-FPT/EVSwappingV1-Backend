namespace Domain.Dtos
{
    public class Enable2FARequest
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

}
