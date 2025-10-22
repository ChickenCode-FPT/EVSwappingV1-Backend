using Application.Batteries.Commands;
using Application.Dtos;
using Domain.Enums;
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
        public async Task<IActionResult> UpdateBatteryStatus([FromBody] UpdateBatteryStatusDto dto)
        {
            await _mediator.Send(new UpdateBatteryStatusCommand(dto.Id));
            return NoContent();
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
        public async Task<IActionResult> CreateBattery([FromBody] CreateBatteryDto dto)
        {


            var command = new CreateBatteryCommand(dto.ModelId, dto.Capacity);

            var id = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetBatteries), new { id }, new { BatteryId = id });
        }


    }
}
