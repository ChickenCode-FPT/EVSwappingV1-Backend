namespace Domain.Dtos
{
    public class Verify2FARequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }

}
