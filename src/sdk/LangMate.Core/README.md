# LangMate.Core

**LangMate.Core** is a lightweight, extensible .NET SDK designed to make working with Ollama-powered local AI models seamless and developer-friendly. It abstracts away the complexity of managing conversations, interacting with Ollama endpoints, and persisting chat history — all while offering resiliency, caching, and extensibility.

## 📦 Installation

To use `LangMate.Core`, install the required NuGet package [![NuGet](https://img.shields.io/nuget/v/LangMate.Core)](https://www.nuget.org/packages/LangMate.Core), or include the project reference in your solution.

```bash
dotnet add package LangMate.Core
```

## ⚙️ Key Features

- 🧠 **Ollama Factory**
    - Abstracts the complexity of working with Ollama AI models
- 🗄️ A clean and extensible persistence layer which provides persisting chat history and conversations, and data caching (with MongoDB).
- ⚙️ **Resiliency Middleware** with:
  - Retry (Polly-based)
  - Timeout
  - Circuit Breaker
  - Serilog and APM support for observe and write ability for logging all requests and errors
- ⚙️ **Exception Handling Middleware**
- ⚙️ **Request Logging Middleware**
- 📉 Graceful degradation with friendly error responses
- 📦 Supports API RESTful and Blazor apps
- 🧑‍💻 Lots of utilities and helpers for simplifying large codes


## 🔧 Service Registration

In your `Program.cs` or inside a service registration method:

```csharp
// Setting Serilog logger
var builder = WebApplication.CreateBuilder(args);
Log.Logger = LangMateLoggingExtensions.AddLangMateLogging(builder.Configuration);

...
// Adding Middleware
builder.Services.AddLangMateCore(builder.Configuration, useApm: false);

...
// Hook Into ASP.NET Core Pipeline
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
app.UseLangMateCore(app.Configuration, loggerFactory);
```

## 🔧 LangMate SDK Sample Usage

Here is an example of using LangMate.Core SDK in a WebAPI controller.
You can find the all methods from **IOllamaFactory** interface as well.

```csharp
    [ApiController]
    [Route("[controller]")]
    public class OllamaController(IOllamaFactory ollamaFactoryProvider) : ControllerBase
    {
        private readonly IOllamaFactory _ollamaFactoryProvider = ollamaFactoryProvider;

        [HttpGet("models")]
        public async Task<IActionResult> GetModelListAsync(string term = "")
        {
            var models = await _ollamaFactoryProvider.GetModelsListAsync();

            return Ok(models.Where(x => x.Name.Contains(term) || x.Description.Contains(term)));
        }

        [HttpGet("models/installed")]
        public async Task<IActionResult> GetLocalModels()
        {
            var models = await _ollamaFactoryProvider.GetAvailableModelsAsync();
            return Ok(models);
        }

        [HttpPut("models/pull/{model}")]
        public async Task<IActionResult> PullModelAsync([Required] string model, CancellationToken cancellationToken)
        {
            var response = _ollamaFactoryProvider.PullModelAsync(model, cancellationToken);
            PullModelResponse? first = null;
            
            await foreach (var progress in response)
            {
                first ??= progress;

                if (first == null || progress.Completed == null || progress.Total == null)
                    continue;

                double completedMB = progress.Completed.Value / 1_000_000.0;
                double totalMB = progress.Total.Value / 1_000_000.0;
                double percent = Math.Min(progress.Completed.Value / (double)progress.Total.Value, 1.0);

                Console.WriteLine($"Downloaded {completedMB:F1}/{totalMB:F1} MB ({percent:P1})");
            }

            await response.EnsureSuccessAsync();

            return Ok(new
            {
                message = $"Downloaded: {first?.Total / 1000000} MB",
                total = $"{first?.Total / 1000000} MB",
            });
        }

        [HttpGet("conversations")]
        public IActionResult GetConversations()
        {
            var models = _ollamaFactoryProvider.GetAllConversations();
            return Ok(models);
        }

        [HttpGet("conversations/{id}")]
        public async Task<IActionResult> GetConversationAsync([Required] string id)
        {
            var models = await _ollamaFactoryProvider.GetConversationAsync(id);
            return Ok(models);
        }

        [HttpPost("conversations")]
        public async Task<IActionResult> StartNewChatCompletionAsync([FromBody] GenerateChatCompletionRequest model, CancellationToken cancellationToken)
        {
            var response = "";
            var results = await _ollamaFactoryProvider.StartNewChatCompletionAsync(model, cancellationToken);
            await foreach (var item in results.response)
            {
                response += item?.Message.Content;
            }

            return Ok(new
            {
                message = response,
                results.conversationId
            });
        }
    }
```

Here is a demo project which contains WebAPI and BlazorUI ChatBot applications working with .NET Aspire and docker.
You can follow the usage of LangMate.Core SDK in both of them based on your requirements. Click the following link to view the GitHub demo and SDK project:

[![Demo Project](https://img.shields.io/badge/Demo%20Project-green)](https://github.com/raminesfahani/LangMate)

## ⚙️ Example `appsettings.json`

```json
{
  ,
  "OllamaOptions": {
    "Model": "llama3.2",
    "Temperature": 0.7,
    "MaxTokens": 1024,
    "Language": "en",
    "Endpoint": "http://localhost:11434/api"
  },
  "MongoDbSettings": {
    "DatabaseName": "LangMateDb"
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "Enrich": [ "FromLogContext", "WithExceptionDetails", "WithCorrelationId" ],
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId} {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  },
  "ResiliencyMiddlewareOptions": {
    "RetryCount": 3,
    "TimeoutSeconds": 10,
    "ExceptionsAllowedBeforeCircuitBreaking": 2,
    "CircuitBreakingDurationSeconds": 30
  }
}
```

---

## 📄 License

MIT License

