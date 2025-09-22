using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace LangMate.Middleware.Middlewares
{
    public class ResiliencyMiddleware(RequestDelegate next, ILogger<ResiliencyMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ResiliencyMiddleware> _logger = logger;

        private readonly AsyncRetryPolicy RetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(1 * retryAttempt), (ex, time) => logger.LogWarning("Retrying request..."));

        private readonly AsyncTimeoutPolicy TimeoutPolicy = Policy
            .TimeoutAsync(10); // 10 sec timeout

        private readonly AsyncCircuitBreakerPolicy CircuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)); // after 5 failures, wait 30s

        public async Task Invoke(HttpContext context)
        {
            var policyWrap = Policy.WrapAsync(RetryPolicy, TimeoutPolicy, CircuitBreakerPolicy);

            try
            {
                await policyWrap.ExecuteAsync(() => _next(context));
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning("Circuit is open. Rejecting request: {Message}", ex.Message);
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Service is temporarily unavailable. Please try again later.");
            }
        }
    }
}
