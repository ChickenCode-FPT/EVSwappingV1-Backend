namespace Application.Dtos
{
    public class CancelReservationRequest
    {
        public int ReservationId { get; set; }
        public string UserId { get; set; }
    }
}
