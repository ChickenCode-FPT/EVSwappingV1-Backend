namespace Domain.Dtos
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public bool RequiresTwoFactor { get; set; }
    }

}
