using Application.Common.Interfaces.Services;
using Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatteryHealthLogsController : ControllerBase
    {
        private readonly IBatteryHealthlogsService _batteryHealthLogsService;
        private readonly ILogger<BatteryHealthLogsController> _logger;

        public BatteryHealthLogsController(
            IBatteryHealthlogsService batteryHealthLogsService,
            ILogger<BatteryHealthLogsController> logger)
        {
            _batteryHealthLogsService = batteryHealthLogsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("API called: GET /api/BatteryHealthLogs");

                var batteryHealthLogs = await _batteryHealthLogsService.GetAll();

                if (batteryHealthLogs == null || !batteryHealthLogs.Any())
                {
                    _logger.LogWarning("No battery health logs found in the system.");
                    return NotFound("No battery health logs found.");
                }

                _logger.LogInformation("Successfully returned {Count} battery health logs.", batteryHealthLogs.Count());
                return Ok(batteryHealthLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving battery health logs.");
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while retrieving battery health logs",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateBatteryHealthLogDto dto)
        {
            try
            {
                var success = await _batteryHealthLogsService.AddAsync(dto);
                if (success)
                    return Ok(new { message = "Battery health log added successfully" });

                return BadRequest("Failed to add log");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding battery health log");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateBatteryHealthLogDto dto)
        {
            var result = await _batteryHealthLogsService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _batteryHealthLogsService.DeleteAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }

    }
}
