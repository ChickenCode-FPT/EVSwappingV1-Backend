namespace Domain.Dtos
{
    public class Verify2FAResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }

}
