using LangMate.Middleware.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace LangMate.Middleware.Tests
{
    public class MiddlewareExtensionsTests
    {
        [Fact]
        public void UseLangMateExceptionHandler_RegistersMiddleware()
        {
            var appMock = new Mock<IApplicationBuilder>();
            appMock.Setup(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                   .Returns(appMock.Object);

            var result = MiddlewareExtensions.UseLangMateExceptionHandler(appMock.Object);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IApplicationBuilder>(result);
        }

        [Fact]
        public void UseLangMateResiliency_RegistersMiddleware()
        {
            var appMock = new Mock<IApplicationBuilder>();
            appMock.Setup(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                   .Returns(appMock.Object);
            var result = MiddlewareExtensions.UseLangMateResiliency(appMock.Object);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IApplicationBuilder>(result);
        }

        [Fact]
        public void UseLangMateRequestLogging_RegistersMiddleware()
        {
            var appMock = new Mock<IApplicationBuilder>();
            appMock.Setup(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                   .Returns(appMock.Object);
            var result = MiddlewareExtensions.UseLangMateRequestLogging(appMock.Object);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IApplicationBuilder>(result);
        }
    }
}