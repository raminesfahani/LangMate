using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace LangMate.Middleware.Middlewares
{
    public class ResiliencyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResiliencyMiddleware> _logger;

        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncTimeoutPolicy _timeoutPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public ResiliencyMiddleware(RequestDelegate next, ILogger<ResiliencyMiddleware> logger)
        {
            _next = next;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    3,
                    attempt => TimeSpan.FromSeconds(attempt),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, "Retry {RetryCount} after {Delay}", retryCount, timeSpan);
                    });

            _timeoutPolicy = Policy
                .TimeoutAsync(20);

            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    5,
                    TimeSpan.FromSeconds(30),
                    onBreak: (ex, ts) =>
                        _logger.LogWarning("Circuit broken: {Message}", ex.Message),
                    onReset: () =>
                        _logger.LogInformation("Circuit closed again."),
                    onHalfOpen: () =>
                        _logger.LogInformation("Circuit in half-open state.")
                );
        }

        public async Task Invoke(HttpContext context)
        {
            // ❗ Skip WebSocket or Blazor server (SignalR) traffic
            if (context.WebSockets.IsWebSocketRequest || context.Request.Path.StartsWithSegments("/_blazor"))
            {
                await _next(context);
                return;
            }

            var policyWrap = Policy.WrapAsync(_retryPolicy, _timeoutPolicy, _circuitBreakerPolicy);

            try
            {
                await policyWrap.ExecuteAsync(() => _next(context));
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning("Circuit is open. Request rejected: {Message}", ex.Message);
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Service temporarily unavailable. Please try again.");
            }
            catch (TimeoutRejectedException ex)
            {
                _logger.LogWarning("Request timed out: {Message}", ex.Message);
                context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                await context.Response.WriteAsync("Request timed out. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in ResiliencyMiddleware");
                throw; // Let other middleware (e.g. error handler) catch it
            }
        }
    }
}
