using Application.Common.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers
{
    [Route("api/batteryModel")]
    [ApiController]
    public class BatteryModelController : ControllerBase
    {
        private readonly IBatteryModelService _batteryModelService;

        public BatteryModelController(IBatteryModelService batteryModelService)
        {
            _batteryModelService = batteryModelService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var batteryModels = await _batteryModelService.GetAll();
                return Ok(batteryModels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the battery models.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var batteryModel = await _batteryModelService.GetById(id);

                if (batteryModel == null)
                {
                    return NotFound($"Battery model with ID {id} not found.");
                }

                return Ok(batteryModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the battery model.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] BatteryModel model)
        {
            if (model == null)
            {
                return BadRequest("Battery model is null.");
            }

            try
            {
                await _batteryModelService.Add(model);
                return CreatedAtAction(nameof(GetById), new { id = model.BatteryModelId }, model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while adding the battery model.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BatteryModel model)
        {
            if (model == null || model.BatteryModelId != id)
            {
                return BadRequest("Battery model is null or ID mismatch.");
            }

            try
            {
                await _batteryModelService.Update(model);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the battery model.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _batteryModelService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the battery model.");
            }
        }
    }
}
