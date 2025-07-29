# Example API Requests

This document provides example requests for testing the SKAIAgents API.

## Basic Examples

### Hello Message
```bash
curl -X POST "http://localhost:5103/api/chat" \
  -H "Content-Type: application/json" \
  -d '{"prompt": "hello"}'
```

Expected Response:
```json
{
  "response": "Hello! I'm your AI assistant powered by Semantic Kernel and Azure AI Foundry. How can I help you today?",
  "sessionId": "generated-session-id",
  "timestamp": "2025-07-29T05:08:36.268Z",
  "success": true,
  "errorMessage": null
}
```

### Help Request
```bash
curl -X POST "http://localhost:5103/api/chat" \
  -H "Content-Type: application/json" \
  -d '{"prompt": "help"}'
```

### Search Query
```bash
curl -X POST "http://localhost:5103/api/chat" \
  -H "Content-Type: application/json" \
  -d '{"prompt": "search", "userId": "user123", "sessionId": "session456"}'
```

### Weather Query
```bash
curl -X POST "http://localhost:5103/api/chat" \
  -H "Content-Type: application/json" \
  -d '{"prompt": "weather"}'
```

### Custom Query
```bash
curl -X POST "http://localhost:5103/api/chat" \
  -H "Content-Type: application/json" \
  -d '{"prompt": "What is Semantic Kernel and how does it work?"}'
```

## Advanced Examples

### Session Management
```bash
# Start a conversation
curl -X POST "http://localhost:5103/api/chat" \
  -H "Content-Type: application/json" \
  -d '{"prompt": "Hello, I need help with Azure AI", "userId": "user123", "sessionId": "conversation-1"}'

# Continue the conversation
curl -X POST "http://localhost:5103/api/chat" \
  -H "Content-Type: application/json" \
  -d '{"prompt": "Tell me more about vectorization", "userId": "user123", "sessionId": "conversation-1"}'
```

### Error Handling
```bash
# Empty prompt (should return error)
curl -X POST "http://localhost:5103/api/chat" \
  -H "Content-Type: application/json" \
  -d '{"prompt": ""}'

# Invalid JSON (should return 400)
curl -X POST "http://localhost:5103/api/chat" \
  -H "Content-Type: application/json" \
  -d '{"invalid": json}'
```

## PowerShell Examples

### Basic Request
```powershell
$headers = @{
    "Content-Type" = "application/json"
}

$body = @{
    prompt = "Hello, how can you help me?"
    userId = "user123"
    sessionId = "session456"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "http://localhost:5103/api/chat" -Method POST -Headers $headers -Body $body
$response | ConvertTo-Json -Depth 3
```

### Health Check
```powershell
$healthResponse = Invoke-RestMethod -Uri "http://localhost:5103/api/chat/health" -Method GET
Write-Host "API Health: $($healthResponse.status)"
```

## Testing Script

Save this as `test-api.sh`:

```bash
#!/bin/bash

API_BASE="http://localhost:5103"

echo "Testing SKAIAgents API..."

# Test health endpoint
echo "1. Health Check:"
curl -s -X GET "$API_BASE/api/chat/health" | jq .

echo -e "\n2. Hello Message:"
curl -s -X POST "$API_BASE/api/chat" \
  -H "Content-Type: application/json" \
  -d '{"prompt": "hello"}' | jq .

echo -e "\n3. Help Request:"
curl -s -X POST "$API_BASE/api/chat" \
  -H "Content-Type: application/json" \
  -d '{"prompt": "help"}' | jq .

echo -e "\n4. Custom Question:"
curl -s -X POST "$API_BASE/api/chat" \
  -H "Content-Type: application/json" \
  -d '{"prompt": "What is Azure AI Foundry?", "userId": "tester", "sessionId": "test-session"}' | jq .

echo -e "\nAPI Testing Complete!"
```

Make it executable and run:
```bash
chmod +x test-api.sh
./test-api.sh
```