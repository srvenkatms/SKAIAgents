using SKAIAgentsAPI.Configuration;
using SKAIAgentsAPI.Models;

namespace SKAIAgentsAPI.Services;

public class DemoAIAgentService : IAIAgentService
{
    private readonly AzureAIConfiguration _config;
    private readonly ILogger<DemoAIAgentService> _logger;

    public DemoAIAgentService(AzureAIConfiguration config, ILogger<DemoAIAgentService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<ChatResponse> ProcessPromptAsync(ChatRequest request)
    {
        try
        {
            _logger.LogInformation("Processing prompt: {Prompt}", request.Prompt);

            // Simulate processing delay
            await Task.Delay(1000);

            // Generate a demo response
            var response = GenerateDemoResponse(request.Prompt);

            return new ChatResponse
            {
                Response = response,
                SessionId = request.SessionId ?? Guid.NewGuid().ToString(),
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing prompt: {Prompt}", request.Prompt);
            return new ChatResponse
            {
                Success = false,
                ErrorMessage = $"Error processing request: {ex.Message}",
                SessionId = request.SessionId
            };
        }
    }

    private string GenerateDemoResponse(string prompt)
    {
        var responses = new Dictionary<string, string>
        {
            { "hello", "Hello! I'm your AI assistant powered by Semantic Kernel and Azure AI Foundry. How can I help you today?" },
            { "weather", "I'd be happy to help you with weather information. In a full implementation, I would use Azure AI Search to find current weather data from your knowledge base." },
            { "search", "I can help you search through your knowledge base using Azure AI Search for vectorization. What would you like to search for?" },
            { "help", "I'm an AI assistant built with:\n- Semantic Kernel AI Agents framework\n- Azure AI Foundry integration\n- Azure AI Search for vectorization\n- Default Azure credentials for authentication\n\nWhat can I help you with?" }
        };

        var lowerPrompt = prompt.ToLowerInvariant();
        
        foreach (var (key, value) in responses)
        {
            if (lowerPrompt.Contains(key))
            {
                return value;
            }
        }

        return $"Thank you for your message: \"{prompt}\"\n\nThis is a demo response from the Semantic Kernel AI Agents API. In a full implementation, this would:\n\n" +
               "1. Connect to Azure AI Foundry using the configured connection string\n" +
               "2. Use Azure AI Search for vectorization and knowledge retrieval\n" +
               "3. Process your query using the configured LLM model\n" +
               "4. Return a contextually relevant response\n\n" +
               "To enable full functionality, please configure your Azure AI Foundry connection string in appsettings.json.";
    }
}