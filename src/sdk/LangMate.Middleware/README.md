# LangMate.Middleware

> Part of the [LangMate.Core](https://github.com/raminesfahani/LangMate) SDK

## 📦 NuGet

[![NuGet](https://img.shields.io/nuget/v/LangMate.Middleware)](https://www.nuget.org/packages/LangMate.Middleware)

**LangMate.Middleware** provides robust, pluggable middleware components for ASP.NET Core applications in the LangMate ecosystem. These middlewares improve resiliency, observability, and error handling—especially in AI-driven apps using local models like Ollama.


## 📦 Installation

To use `LangMate.Middleware`, install the required NuGet package, or include the project reference in your solution.

```bash
dotnet add package LangMate.Middleware
```

## ✨ Middlewares and Features

- **Resiliency Middleware** with:
  - Retry (Polly-based)
  - Timeout
  - Circuit Breaker
  - Serilog and APM support for observe and write ability for logging all requests and errors
- **Exception Handling Middleware**
- **Request Logging Middleware**
- 📉 Graceful degradation with friendly error responses
- 🧩 Plug-and-play into any ASP.NET Core app


# 🛡️ Resiliency Middleware

The **Resiliency Middleware** is a centralized, fault-tolerant HTTP request wrapper built using the [Polly](https://github.com/App-vNext/Polly) resilience framework. It enhances the reliability and responsiveness of your ASP.NET Core application by automatically handling transient failures, long-running requests, and service degradation.

This middleware is designed for AI-driven or service-heavy applications, such as LangMate, that depend on local/remote models, file uploads, and dynamic APIs.

---

## 🔧 What It Does

Wraps every HTTP request in a resilient policy pipeline:

| Policy             | Behavior                                                                 |
|--------------------|--------------------------------------------------------------------------|
| **Retry**          | Retries failed requests up to **3 times** with exponential backoff.       |
| **Timeout**        | Cancels requests that exceed **10 seconds**, returns **504 Gateway Timeout**. |
| **Circuit Breaker**| Opens circuit after **2 consecutive failures**, blocks requests for **30s**. |

> Blazor SignalR (`/_blazor`) and WebSocket requests are automatically excluded to avoid interfering with real-time behavior.

---

## ⚙️ Behavior at Runtime

- ✅ Automatically **retries** transient errors (e.g., exceptions from external calls).
- ⏱ Times out long requests and fails gracefully.
- 🔌 Breaks the circuit when failures reach a threshold.
- 📉 Returns user-friendly responses:
  - `503 Service Unavailable` (circuit open)
  - `504 Gateway Timeout` (timeout triggered)
- 🧠 Logs events like retries, breaks, resets, and errors.

---



## ⚙️ Example Resiliency Middleware `appsettings.json`

```json
{
  "ResiliencyMiddlewareOptions": {
    "RetryCount": 3,
    "TimeoutSeconds": 10,
    "ExceptionsAllowedBeforeCircuitBreaking": 2,
    "CircuitBreakingDurationSeconds": 30
  }
}
```

---

### Serilog Logger

- Enrich logs with **context** and **exceptions**.  
- Add **correlation IDs** for request tracing.
- Load settings from your `appsettings.json`.  

---


## ⚙️ Example Serilog `appsettings.json`

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.Elasticsearch" ],
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
      },
      // optional if you want to save the logs into a file
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 14,
          "shared": true
        }
      },
      // optional if using APM
      {
        "Name": "Elasticsearch",
        "Args": {
          "nodeUris": "http://localhost:9200",
          "indexFormat": "myapp-logs-{0:yyyy.MM}",
          "autoRegisterTemplate": true,
          "autoRegisterTemplateVersion": "ESv7"
        }
      }
    ]
  },
  // optional if using APM
  "ElasticApm": {
    "ServerUrls": "http://localhost:8200",
    "ServiceName": "MyApp.Service",
    "Environment": "Development",
    "SecretToken": "",
    "TransactionSampleRate": 1.0
  }
}
```

---

## 🔧 Service Registration

In your `Program.cs` or inside a service registration method (already is done in LangMate.Core):

```csharp
// Setting Serilog logger
var builder = WebApplication.CreateBuilder(args);
Log.Logger = LangMateLoggingExtensions.AddLangMateLogging(builder.Configuration);

...
// Adding Middleware
services.AddLangMateMiddleware(configuration, useApm: useApm);

...
// Hook Into ASP.NET Core Pipeline
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
app.UseLangMateMiddleware(configuration, loggerFactory);
```

## 📄 License

MIT License