using SKAIAgentsAPI.Configuration;
using SKAIAgentsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configure Azure AI settings
var azureAIConfig = builder.Configuration.GetSection(AzureAIConfiguration.SectionName).Get<AzureAIConfiguration>();
if (azureAIConfig == null)
{
    azureAIConfig = new AzureAIConfiguration();
    builder.Configuration.GetSection(AzureAIConfiguration.SectionName).Bind(azureAIConfig);
}
builder.Services.AddSingleton(azureAIConfig);

// Register AI Agent service - using demo service for now
// In production, uncomment the line below and configure Azure AI Foundry properly
// builder.Services.AddScoped<IAIAgentService, AIAgentService>();
builder.Services.AddScoped<IAIAgentService, DemoAIAgentService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "SKAIAgents API", 
        Version = "v1",
        Description = "API for Semantic Kernel AI Agents with Azure AI Foundry integration"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
