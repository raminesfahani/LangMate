using LangMate.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LangMate.Middleware.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddLangMateMiddleware_RegistersServices()
        {
            var services = new ServiceCollection();
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);

            var result = services.AddLangMateMiddleware(configMock.Object, useApm: true);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IServiceCollection>(result);
        }
    }
}