using LangMate.Abstractions.Options;
using LangMate.Middleware.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System.Threading.Tasks;
using Xunit;

namespace LangMate.Middleware.Tests
{
    public class ResiliencyMiddlewareTests
    {
        [Fact]
        public async Task Invoke_SkipsWebSocketRequest()
        {
            var context = new DefaultHttpContext();
            context.Features.Set(new Mock<IHttpWebSocketFeature>().Object);

            var nextCalled = false;
            RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };

            var loggerMock = new Mock<ILogger<ResiliencyMiddleware>>();
            var options = Options.Create(new ResiliencyMiddlewareOptions
            {
                RetryCount = 1,
                TimeoutSeconds = 1,
                ExceptionsAllowedBeforeCircuitBreaking = 1,
                CircuitBreakingDurationSeconds = 1
            });

            var middleware = new ResiliencyMiddleware(next, loggerMock.Object, options);

            await middleware.Invoke(context);

            Assert.True(nextCalled);
        }

        [Fact]
        public async Task Invoke_SkipsBlazorSignalRRequest()
        {
            var context = new DefaultHttpContext();
            context.Request.Path = "/_blazor";

            var nextCalled = false;
            RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };

            var loggerMock = new Mock<ILogger<ResiliencyMiddleware>>();
            var options = Options.Create(new ResiliencyMiddlewareOptions
            {
                RetryCount = 1,
                TimeoutSeconds = 1,
                ExceptionsAllowedBeforeCircuitBreaking = 1,
                CircuitBreakingDurationSeconds = 1
            });

            var middleware = new ResiliencyMiddleware(next, loggerMock.Object, options);

            await middleware.Invoke(context);

            Assert.True(nextCalled);
        }

        [Fact]
        public async Task Invoke_BrokenCircuitException_Returns503()
        {
            var context = new DefaultHttpContext();
            RequestDelegate next = ctx => throw new BrokenCircuitException();

            var loggerMock = new Mock<ILogger<ResiliencyMiddleware>>();
            var options = Options.Create(new ResiliencyMiddlewareOptions
            {
                RetryCount = 3,
                TimeoutSeconds = 30,
                ExceptionsAllowedBeforeCircuitBreaking = 3,
                CircuitBreakingDurationSeconds = 30
            });

            var middleware = new ResiliencyMiddleware(next, loggerMock.Object, options);

            await middleware.Invoke(context);

            Assert.Equal(StatusCodes.Status503ServiceUnavailable, context.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_TimeoutRejectedException_Returns504()
        {
            var context = new DefaultHttpContext();
            RequestDelegate next = ctx => throw new TimeoutRejectedException();

            var loggerMock = new Mock<ILogger<ResiliencyMiddleware>>();
            var options = Options.Create(new ResiliencyMiddlewareOptions
            {
                RetryCount = 0,
                TimeoutSeconds = 1,
                ExceptionsAllowedBeforeCircuitBreaking = 1,
                CircuitBreakingDurationSeconds = 1
            });

            var middleware = new ResiliencyMiddleware(next, loggerMock.Object, options);

            await middleware.Invoke(context);

            Assert.Equal(StatusCodes.Status504GatewayTimeout, context.Response.StatusCode);
        }
    }
}