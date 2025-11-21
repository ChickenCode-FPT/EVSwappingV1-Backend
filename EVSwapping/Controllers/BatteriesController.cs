using Application.Batteries.Commands;
using Application.Dtos.Battery;
using Application.StationInventories.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [ApiController]
    [Route("api/batteries")]
    public class BatteriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BatteriesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetBatteries()
        {
            var result = await _mediator.Send(new Application.Batteries.Queries.GetBatteriesQuery());
            return Ok(result);
        }

        [HttpGet("/api/battery/{id}")]
        public async Task<IActionResult> GetBatteriesId(int id)
        {
            var result = await _mediator.Send(new Application.Batteries.Queries.GetBatteriesIDQuery(id));
            return Ok(result);
        }

        //[HttpGet]
        //[Route("status/{status}")]
        //public async Task<IActionResult> GetBatteriesByStatus(string status)
        //{
        //    var result = await _mediator.Send(new Application.Batteries.Queries.GetBatteriesByStatusQuery(status));
        //    return Ok(result);
        //}

        [HttpPut("status")]
        public async Task<IActionResult> UpdateBateryStatus([FromBody] UpdateBatteryStatusCommand command)
        {

            try
            {
                var result = await _mediator.Send(command);
                return Ok(new { message = "Battery successfully.", batteryId = result });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("duplicate"))
                {
                    return BadRequest(new { message = ex.Message });
                }

                return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBattery([FromBody] UpdateBatteryCommand command)
        {
            if (command == null)
                return BadRequest("Invalid data.");

            var updatedBatteryId = await _mediator.Send(command);

            return Ok(new { BatteryId = updatedBatteryId });
        }

        [HttpPost]
        public async Task<IActionResult> AddBattery([FromBody] CreateBatteryCommand command)
        {

            try
            {
                var result = await _mediator.Send(command);
                return Ok(new { message = "Battery added successfully.", batteryId = result });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("duplicate"))
                {
                    return BadRequest(new { message = ex.Message });
                }

                return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
            }
        }
    }
}
