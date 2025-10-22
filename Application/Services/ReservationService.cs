using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly IMapper _mapper;

        public ReservationService(IReservationRepository reservationRepo, IMapper mapper)
        {
            _reservationRepo = reservationRepo;
            _mapper = mapper;
        }

        public async Task<ReservationDto> CreateReservation(CreateReservationRequest request)
        {
            var reservation = new Reservation
            {
                UserId = request.UserId,
                StationId = request.StationId,
                VehicleId = request.VehicleId,
                ReservedFrom = request.ReservedFrom,
                ReservedTo = request.ReservedTo,
                ReservedBatteryModelId = request.ReservedBatteryModelId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _reservationRepo.Add(reservation);
            return _mapper.Map<ReservationDto>(reservation);
        }

        public async Task CancelReservation(CancelReservationRequest request)
        {
            var res = await _reservationRepo.GetById(request.ReservationId);
            if (res == null || res.UserId != request.UserId)
                throw new UnauthorizedAccessException("Bạn không thể hủy đặt lịch này.");

            await _reservationRepo.Cancel(request.ReservationId);
        }

        public async Task<IEnumerable<ReservationDto>> GetReservationsByUser(string userId)
        {
            var reservations = await _reservationRepo.GetByUserId(userId);
            return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
        }
    }
}
