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

