using Application.Common.Interfaces.Services;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest request)
        {
            var reservation = await _reservationService.CreateReservation(request);
            return Ok(reservation);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CancelReservation(int id, [FromQuery] string userId)
        {
            await _reservationService.CancelReservation(new CancelReservationRequest
            {
                ReservationId = id,
                UserId = userId
            });

            return NoContent();
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<ReservationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var reservations = await _reservationService.GetReservationsByUser(userId);
            return Ok(reservations);
        }
    }
}
