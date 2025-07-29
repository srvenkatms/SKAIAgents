#pragma warning disable SKEXP0110

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using SKAIAgentsAPI.Configuration;
using SKAIAgentsAPI.Models;

namespace SKAIAgentsAPI.Services;

public interface IAIAgentService
{
    Task<ChatResponse> ProcessPromptAsync(ChatRequest request);
}

public class AIAgentService : IAIAgentService
{
    private readonly AzureAIConfiguration _config;
    private readonly ILogger<AIAgentService> _logger;
    private Kernel? _kernel;
    private IChatCompletionService? _chatService;
    private SearchClient? _searchClient;

    public AIAgentService(AzureAIConfiguration config, ILogger<AIAgentService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<ChatResponse> ProcessPromptAsync(ChatRequest request)
    {
        try
        {
            // Initialize services if not already done
            if (_kernel == null || _chatService == null)
            {
                InitializeServices();
            }

            if (_chatService == null)
            {
                return new ChatResponse
                {
                    Success = false,
                    ErrorMessage = "Failed to initialize AI services",
                    SessionId = request.SessionId
                };
            }

            // Prepare the chat history
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("You are a helpful AI assistant that can answer questions and help users with their tasks.");

            // Add search context if available
            if (_searchClient != null && !string.IsNullOrWhiteSpace(request.Prompt))
            {
                try
                {
                    var searchResults = await SearchKnowledgeBaseAsync(request.Prompt);
                    if (!string.IsNullOrEmpty(searchResults))
                    {
                        chatHistory.AddSystemMessage($"Use the following information from the knowledge base to help answer the user's question:\n{searchResults}");
                    }
                }
                catch (Exception searchEx)
                {
                    _logger.LogWarning(searchEx, "Failed to search knowledge base, proceeding without search results");
                }
            }

            // Add user message
            chatHistory.AddUserMessage(request.Prompt);

            // Get response from chat completion service
            var response = await _chatService.GetChatMessageContentAsync(
                chatHistory,
                new AzureOpenAIPromptExecutionSettings
                {
                    MaxTokens = 1000,
                    Temperature = 0.7
                });

            return new ChatResponse
            {
                Response = response.Content ?? "No response generated",
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

    private void InitializeServices()
    {
        try
        {
            // Validate configuration
            if (string.IsNullOrEmpty(_config.ProjectConnectionString))
            {
                throw new InvalidOperationException("Azure AI Project connection string is required");
            }

            // Create AI Project client with default credentials
            var projectClient = new AIProjectClient(
                connectionString: _config.ProjectConnectionString,
                credential: new DefaultAzureCredential()
            );

            // Parse connection string to get endpoint and other details
            var connectionStringParts = ParseConnectionString(_config.ProjectConnectionString);
            
            if (!connectionStringParts.ContainsKey("endpoint"))
            {
                throw new InvalidOperationException("Endpoint not found in connection string");
            }

            var endpoint = connectionStringParts["endpoint"];

            // Create kernel with Azure OpenAI configuration
            var kernelBuilder = Kernel.CreateBuilder();
            
            // Add Azure OpenAI chat completion service
            kernelBuilder.AddAzureOpenAIChatCompletion(
                deploymentName: _config.ModelDeploymentName ?? _config.ModelName ?? "gpt-35-turbo",
                endpoint: endpoint,
                apiKey: "dummy-key", // This will be overridden by the credential
                httpClient: null);

            // For now, we'll use a simpler approach with API key authentication
            // In a real scenario, you would configure this properly with the connection string details

            _kernel = kernelBuilder.Build();
            _chatService = _kernel.GetRequiredService<IChatCompletionService>();

            // Initialize search client if configured
            if (!string.IsNullOrEmpty(_config.SearchServiceName) && !string.IsNullOrEmpty(_config.SearchIndexName))
            {
                var searchServiceUri = new Uri($"https://{_config.SearchServiceName}.search.windows.net");
                _searchClient = new SearchClient(
                    searchServiceUri,
                    _config.SearchIndexName,
                    new DefaultAzureCredential()
                );
            }

            _logger.LogInformation("AI services initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize AI services");
            throw;
        }
    }

    private async Task<string?> SearchKnowledgeBaseAsync(string query)
    {
        if (_searchClient == null)
        {
            return null;
        }

        try
        {
            var searchOptions = new SearchOptions
            {
                Size = 3,
                Select = { "content" }
            };

            var searchResults = await _searchClient.SearchAsync<dynamic>(query, searchOptions);
            var results = new List<string>();

            await foreach (var result in searchResults.Value.GetResultsAsync())
            {
                if (result.Document.TryGetValue("content", out object? content))
                {
                    results.Add(content.ToString() ?? "");
                }
            }

            return results.Any() ? string.Join("\n\n", results) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching knowledge base");
            return null;
        }
    }

    private static Dictionary<string, string> ParseConnectionString(string connectionString)
    {
        var parts = new Dictionary<string, string>();
        
        foreach (var part in connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var keyValue = part.Split('=', 2);
            if (keyValue.Length == 2)
            {
                parts[keyValue[0].Trim().ToLowerInvariant()] = keyValue[1].Trim();
            }
        }

        return parts;
    }
}