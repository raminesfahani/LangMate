using LangMate.Abstractions.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace LangMate.Middleware.Middlewares
{
    public class ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;
        private readonly IHostEnvironment _env = env;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var (statusCode, title) = MapExceptionToStatusCodeAndTitle(ex);
                var traceId = context.TraceIdentifier;

                _logger.LogError(ex,
                    "[{TraceId}] Unhandled exception: {ExceptionType} at {Path} => {Message}",
                    traceId,
                    ex.GetType().Name,
                    context.Request.Path,
                    ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Type = "https://httpstatuses.com/" + statusCode,
                    Title = title,
                    Status = statusCode,
                    Detail = _env.IsDevelopment() ? ex.ToString() : "",
                    Instance = context.Request.Path
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/problem+json";

                var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });

                await context.Response.WriteAsync(json);
            }
        }

        private static (int statusCode, string title) MapExceptionToStatusCodeAndTitle(Exception ex)
        {
            return ex switch
            {
                // ⚠️ Client errors
                ArgumentNullException => (400, "A required argument was null."),
                ArgumentException => (400, "An invalid argument was provided."),
                FormatException => (400, "Invalid input format."),
                ValidationException => (400, "Validation failed."),
                InvalidOperationException => (400, "The operation was invalid."),

                // ⛔ Unauthorized / Authentication
                UnauthorizedAccessException => (401, "Unauthorized access."),

                // ❌ Not Found / Missing
                KeyNotFoundException => (404, "Resource not found."),
                FileNotFoundException => (404, "Requested file was not found."),
                DirectoryNotFoundException => (404, "Requested directory was not found."),

                // 🗃️ Persistence / Database
                NotSupportedException => (405, "The operation is not supported."),
                NotImplementedException => (501, "This functionality is not implemented."),

                // 💣 Critical internal errors
                OutOfMemoryException => (500, "The server ran out of memory."),
                StackOverflowException => (500, "Stack overflow occurred."),
                TimeoutException => (504, "The request timed out."),

                // 🌐 Network issues
                HttpRequestException => (503, "A network error occurred while processing the request."),
                TaskCanceledException => (408, "The request was canceled or timed out."),

                // Default fallback
                _ => (500, "An unexpected error occurred.")
            };
        }
    }
}
