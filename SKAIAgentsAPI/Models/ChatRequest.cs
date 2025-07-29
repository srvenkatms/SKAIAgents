namespace SKAIAgentsAPI.Models;

public class ChatRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
}