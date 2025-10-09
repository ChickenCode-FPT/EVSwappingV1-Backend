using Application.Common.Interfaces.Services;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportTicketController : ControllerBase
    {
        private readonly ISupportTicketService _ticketService;

        public SupportTicketController(ISupportTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateSupportTicketRequest request)
        {
            var ticket = await _ticketService.CreateTicketAsync(request);
            return CreatedAtAction(nameof(GetTicketById), new { ticketId = ticket.TicketId }, ticket);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTicketById(long ticketId)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTicketsByUserId(string userId)
        {
            var tickets = await _ticketService.GetTicketsByUserIdAsync(userId);
            return Ok(tickets);
        }

        [HttpGet("station/{stationId}")]
        public async Task<IActionResult> GetTicketsByStationId(int stationId)
        {
            var tickets = await _ticketService.GetTicketsByStationIdAsync(stationId);
            return Ok(tickets);
        }

        [HttpPut("{ticketId}/status")]
        public async Task<IActionResult> UpdateTicketStatus(long ticketId, [FromBody] string newStatus)
        {
            var ticket = await _ticketService.UpdateTicketStatusAsync(ticketId, newStatus);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        [HttpPut("{ticketId}/close")]
        public async Task<IActionResult> CloseTicket(long ticketId)
        {
            await _ticketService.CloseTicketAsync(ticketId);
            return NoContent();
        }
    }
}
