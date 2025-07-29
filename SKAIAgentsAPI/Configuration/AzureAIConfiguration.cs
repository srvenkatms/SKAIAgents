namespace SKAIAgentsAPI.Configuration;

public class AzureAIConfiguration
{
    public const string SectionName = "AzureAI";
    
    public string ProjectConnectionString { get; set; } = string.Empty;
    public string? ModelDeploymentName { get; set; }
    public string? ModelName { get; set; } = "gpt-35-turbo";
    public string? SearchServiceName { get; set; }
    public string? SearchIndexName { get; set; }
}