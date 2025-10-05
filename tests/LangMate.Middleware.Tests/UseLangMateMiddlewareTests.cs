using LangMate.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LangMate.Middleware.Tests
{
    public class UseLangMateMiddlewareTests
    {
        [Fact]
        public void UseLangMateMiddleware_RegistersMiddlewares()
        {
            var appMock = new Mock<IApplicationBuilder>();
            appMock.Setup(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>())).Returns(appMock.Object);

            var configMock = new Mock<IConfiguration>();
            var loggerFactoryMock = new Mock<ILoggerFactory>();

            var result = appMock.Object.UseLangMateMiddleware(configMock.Object, loggerFactoryMock.Object);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IApplicationBuilder>(result);
        }
    }
}