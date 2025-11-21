using Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BatteryController : ControllerBase
    {
        private readonly IBatteryService _batteryService;

        public BatteryController(IBatteryService batteryService)
        {
            _batteryService = batteryService;
        }

        [HttpGet("stations/{stationId}/batteries/available-outgoing")]
        public async Task<IActionResult> GetAvailableOutgoing(int stationId, [FromQuery] int? batteryModelId = null)
        {
            var result = await _batteryService.GetAvailableOutgoing(stationId, batteryModelId);
            return Ok(result);
        }

        [HttpGet("users/{userId}/batteries/inuse")]
        public async Task<IActionResult> GetIncomingInUse(string userId)
        {
            var result = await _batteryService.GetIncomingForUser(userId);
            return Ok(result);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableBatteries([FromQuery] int stationId, [FromQuery] int? batteryModelId = null)
        {
            var batteries = await _batteryService.GetAvailableBatteries(stationId, batteryModelId);
            return Ok(batteries);
        }
    }
}
