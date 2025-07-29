using Microsoft.AspNetCore.Mvc;
using SKAIAgentsAPI.Models;
using SKAIAgentsAPI.Services;

namespace SKAIAgentsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IAIAgentService _aiAgentService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IAIAgentService aiAgentService, ILogger<ChatController> logger)
    {
        _aiAgentService = aiAgentService;
        _logger = logger;
    }

    /// <summary>
    /// Process user prompt and return AI agent response
    /// </summary>
    /// <param name="request">Chat request containing the user prompt</param>
    /// <returns>AI agent response</returns>
    [HttpPost]
    public async Task<ActionResult<ChatResponse>> ProcessPrompt([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            return BadRequest(new ChatResponse
            {
                Success = false,
                ErrorMessage = "Prompt cannot be empty",
                SessionId = request.SessionId
            });
        }

        try
        {
            _logger.LogInformation("Processing prompt for user: {UserId}, session: {SessionId}", 
                request.UserId, request.SessionId);

            var response = await _aiAgentService.ProcessPromptAsync(request);
            
            if (response.Success)
            {
                _logger.LogInformation("Successfully processed prompt for session: {SessionId}", response.SessionId);
                return Ok(response);
            }
            else
            {
                _logger.LogWarning("Failed to process prompt: {ErrorMessage}", response.ErrorMessage);
                return StatusCode(500, response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing prompt");
            return StatusCode(500, new ChatResponse
            {
                Success = false,
                ErrorMessage = "An unexpected error occurred",
                SessionId = request.SessionId
            });
        }
    }

    /// <summary>
    /// Health check endpoint for the chat service
    /// </summary>
    /// <returns>Service status</returns>
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
}