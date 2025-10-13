using Application.Common.Interfaces.Services;
using Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest request)
        {
            var reservation = await _reservationService.CreateReservation(request);
            return Ok(reservation);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            await _reservationService.CancelReservation(new CancelReservationRequest
            {
                ReservationId = id
            });

            return NoContent();
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyReservations()
        {
            var reservations = await _reservationService.GetMyReservations();
            return Ok(reservations);
        }
    }
}
