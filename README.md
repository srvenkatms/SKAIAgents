# SKAIAgents API

An API built with **Semantic Kernel AI Agents** and **Azure AI Foundry** integration for processing prompts and returning AI-powered responses.

## Features

- **Semantic Kernel AI Agents Framework**: Built using Microsoft's Semantic Kernel for AI orchestration
- **Azure AI Foundry Integration**: Connects to Azure AI Foundry Hub Projects using connection strings
- **Azure AI Search for Vectorization**: Supports knowledge base search and retrieval
- **Default Azure Credentials**: Uses managed identity and default credentials (no API keys required)
- **RESTful API**: Clean REST endpoints for chat interactions
- **Swagger Documentation**: Built-in API documentation and testing interface

## Architecture

```
Client Request → API Controller → AI Agent Service → Azure AI Foundry + Azure AI Search → Response
```

## Configuration

Configure the API by updating `appsettings.json`:

```json
{
  "AzureAI": {
    "ProjectConnectionString": "your-azure-ai-foundry-connection-string",
    "ModelDeploymentName": "gpt-35-turbo",
    "ModelName": "gpt-35-turbo",
    "SearchServiceName": "your-search-service-name",
    "SearchIndexName": "your-search-index-name"
  }
}
```

## API Endpoints

### POST /api/chat
Process a user prompt and return an AI response.

**Request Body:**
```json
{
  "prompt": "Your question or message here",
  "userId": "optional-user-id",
  "sessionId": "optional-session-id"
}
```

**Response:**
```json
{
  "response": "AI assistant response",
  "sessionId": "generated-or-provided-session-id",
  "timestamp": "2025-07-29T05:08:36.268Z",
  "success": true,
  "errorMessage": null
}
```

### GET /api/chat/health
Health check endpoint for monitoring.

**Response:**
```json
{
  "status": "Healthy",
  "timestamp": "2025-07-29T05:08:47.189Z"
}
```

## Usage Examples

### Using curl

```bash
# Basic chat request
curl -X POST "http://localhost:5103/api/chat" \
  -H "Content-Type: application/json" \
  -d '{"prompt": "Hello, how can you help me?"}'

# Chat with session tracking
curl -X POST "http://localhost:5103/api/chat" \
  -H "Content-Type: application/json" \
  -d '{
    "prompt": "What is Semantic Kernel?",
    "userId": "user123",
    "sessionId": "session456"
  }'

# Health check
curl -X GET "http://localhost:5103/api/chat/health"
```

### Using PowerShell

```powershell
# Basic chat request
$body = @{
    prompt = "Hello, how can you help me?"
    userId = "user123"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5103/api/chat" -Method POST -Body $body -ContentType "application/json"
```

### Using JavaScript/Node.js

```javascript
const response = await fetch('http://localhost:5103/api/chat', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    prompt: 'Hello, how can you help me?',
    userId: 'user123',
    sessionId: 'session456'
  })
});

const result = await response.json();
console.log(result);
```

### Using Python

```python
import requests

url = "http://localhost:5103/api/chat"
payload = {
    "prompt": "Hello, how can you help me?",
    "userId": "user123",
    "sessionId": "session456"
}

response = requests.post(url, json=payload)
result = response.json()
print(result)
```

## Development Setup

1. **Prerequisites:**
   - .NET 8.0 SDK
   - Azure subscription with AI Foundry access

2. **Build and Run:**
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```

3. **Access Swagger UI:**
   Open `http://localhost:5103/swagger` in your browser

## Azure AI Foundry Configuration

To enable full functionality:

1. Create an Azure AI Foundry Hub and Project
2. Copy the connection string from the Azure portal
3. Update `appsettings.json` with your connection string
4. Optionally configure Azure AI Search for knowledge base functionality
5. Ensure your application has appropriate Azure permissions

## Key Components

- **Models/**: Request and response data models
- **Services/**: AI Agent service implementations
- **Controllers/**: REST API controllers
- **Configuration/**: Configuration classes for Azure AI settings

## Authentication

The API uses Azure Default Credentials, which automatically handles:
- Managed Identity (when deployed to Azure)
- Azure CLI credentials (for local development)
- Visual Studio credentials
- Environment variables

No explicit API keys required in the application code.

## Demo Mode

The current implementation includes a demo service that simulates AI responses for development and testing purposes. To enable full Azure AI Foundry integration, configure the connection string and uncomment the production service registration in `Program.cs`.