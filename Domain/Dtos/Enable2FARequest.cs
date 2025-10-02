namespace Domain.Dtos
{
    public class Enable2FARequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }

}
