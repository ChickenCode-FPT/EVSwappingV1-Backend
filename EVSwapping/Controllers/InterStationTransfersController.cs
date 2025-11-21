using Application.Common.Interfaces.Services;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EVSwapping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterStationTransfersController : ControllerBase
    {
        private readonly IInterStationTransferService _service;

        public InterStationTransfersController(IInterStationTransferService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTransfer([FromBody] CreateTransferDto dto)
        {
            var transfer = await _service.CreateTransferAsync(dto);
            return Ok(transfer);
        }

        [HttpPost("{transferId}/approve")]
        public async Task<IActionResult> ApproveTransfer(long transferId, [FromBody] ApproveTransferDto dto)
        {
            var success = await _service.ApproveTransferAsync(transferId, dto.ApprovedByUserId);
            return success ? Ok("Transfer approved.") : BadRequest("Invalid transfer or status.");
        }

        [HttpPost("{transferId}/complete")]
        public async Task<IActionResult> CompleteTransfer(long transferId, [FromBody] CompleteInterStationTransfer completeInterStationTransfer)
        {
            var success = await _service.CompleteTransferAsync(transferId, completeInterStationTransfer);
            return success ? Ok(new { message = "Transfer completed." })
            : BadRequest(new { message = "Invalid transfer or status." });
        }

        [HttpGet("station/{stationId}")]
        public async Task<IActionResult> GetTransfers(int stationId)
        {
            var transfers = await _service.GetTransfersByStationAsync(stationId);
            return Ok(transfers);
        }

        [HttpGet("outgoing")]
        public async Task<IActionResult> GetOutgoingTransfers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var result = await _service.GetOutgoingTransfersAsync(userId);
            return Ok(result);
        }

        [HttpGet("incoming")]
        public async Task<IActionResult> GetIncomingTransfers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.GetIncomingTransfersAsync(userId);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<InterStationTransferAdminDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllTransfers()
        {
            try
            {
                var result = await _service.GetAllTransfersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi nội bộ khi lấy dữ liệu.");
            }
        }

        [HttpGet("station/{stationId}/available-slots")]
        public async Task<IActionResult> GetAvailableSlots(int stationId)
        {
            var slots = await _service.GetAvaiableSlot(stationId);
            return Ok(slots);
        }
    }
}
