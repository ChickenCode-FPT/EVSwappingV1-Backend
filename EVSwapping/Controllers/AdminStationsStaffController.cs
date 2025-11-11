using Application.Common.Interfaces.Services;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminStationsStaffController : ControllerBase
    {
        private readonly IStationStaffService _stationStaffService;

        public AdminStationsStaffController(IStationStaffService stationStaffService)
        {
            _stationStaffService = stationStaffService;
        }

        [HttpPost("{stationId}/assign-staff")]
        public async Task<IActionResult> AssignStaff(int stationId, [FromBody] AssignStaffDto dto)
        {
            await _stationStaffService.AssignStaffAsync(stationId, dto.UserId, dto.Role);
            return Ok(new { message = "Đã gán nhân viên cho trạm." });
        }

        [HttpGet("{stationId}/staffs")]
        public async Task<IActionResult> GetStationStaffs(int stationId)
        {
            var staffs = await _stationStaffService.GetStationStaffsAsync(stationId);
            return Ok(staffs);
        }

        [HttpDelete("staff/{stationStaffId}")]
        public async Task<IActionResult> RemoveStaff(int stationStaffId)
        {
            await _stationStaffService.RemoveStaffAsync(stationStaffId);
            return Ok(new { message = "Đã xóa nhân viên khỏi trạm." });
        }

        [HttpPut("staff/{stationStaffId}/deactivate")]
        public async Task<IActionResult> DeactivateStaff(int stationStaffId)
        {
            await _stationStaffService.DeactivateStaffAsync(stationStaffId);
            return Ok(new { message = "Đã vô hiệu hóa nhân viên trong trạm." });
        }

        [HttpGet("by-code/{stationCode}/staffs")]
        public async Task<IActionResult> GetStationStaffsByCode(string stationCode)
        {
            var staffs = await _stationStaffService.GetStationStaffsByCodeAsync(stationCode);
            if (staffs == null || !staffs.Any())
                return NotFound(new { message = "No staff found for this station code." });

            return Ok(staffs);
        }

        [HttpGet("by-name/{stationName}/staffs")]
        public async Task<IActionResult> GetStationStaffsByNamne(string stationName)
        {
            var staffs = await _stationStaffService.GetStationStaffsByNameAsync(stationName);
            if (staffs == null || !staffs.Any())
                return NotFound(new { message = "No staff found for this station name." });

            return Ok(staffs);
        }

    }
}
