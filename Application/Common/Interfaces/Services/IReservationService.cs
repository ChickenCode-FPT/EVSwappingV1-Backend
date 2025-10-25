using Application.Dtos;

namespace Application.Common.Interfaces.Services
{
    public interface IReservationService
    {
        Task<ReservationDto> CreateReservation(CreateReservationRequest request);
        Task CancelReservation(CancelReservationRequest request);
        Task<IEnumerable<ReservationDto>> GetMyReservations();
    }
}
