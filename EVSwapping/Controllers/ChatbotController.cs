using Application.Common.Interfaces.Services;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EVSwapping.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatbotController : ControllerBase
{
    private readonly IChatbotService _chatbotService;

    public ChatbotController(IChatbotService chatbotService)
    {
        _chatbotService = chatbotService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ChatbotRequest request)
    {
        if (request.History == null || request.History.Count == 0)
        {
            return BadRequest("History cannot be empty.");
        }

        try
        {
            var response = await _chatbotService.SendMessageAsync(request.History);

            return Ok(new { response });
        }
        catch
        {
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }
}
