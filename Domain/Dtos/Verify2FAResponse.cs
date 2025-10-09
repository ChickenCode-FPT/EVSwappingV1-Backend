namespace Domain.Dtos
{
    public class Verify2FAResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiry { get; set; }
    }

}
