using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LangMate.Middleware.Middlewares
{
    public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<RequestLoggingMiddleware> _logger = logger;

        public async Task Invoke(HttpContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path;
            var traceId = context.TraceIdentifier;

            _logger.LogInformation("➡️ {Method} {Path} [TraceId: {TraceId}]", method, path, traceId);

            await _next(context);
        }
    }
}
