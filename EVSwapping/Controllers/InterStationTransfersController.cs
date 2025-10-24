using Application.Common.Interfaces.Services;
using Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> CompleteTransfer(long transferId)
        {
            var success = await _service.CompleteTransferAsync(transferId);
            return success ? Ok("Transfer completed.") : BadRequest("Invalid transfer or status.");
        }

        [HttpGet("station/{stationId}")]
        public async Task<IActionResult> GetTransfers(int stationId)
        {
            var transfers = await _service.GetTransfersByStationAsync(stationId);
            return Ok(transfers);
        }
    }
}
