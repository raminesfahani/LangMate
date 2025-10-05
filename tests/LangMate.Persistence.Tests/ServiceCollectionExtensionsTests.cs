using LangMate.Persistence;
using LangMate.Abstractions.Contracts;
using LangMate.Abstractions.Abstracts.Settings;
using LangMate.Abstractions.Abstracts.Persistence;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace LangMate.Persistence.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddLangMateMemoryCache_RegistersMemoryCacheAndProvider()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var result = services.AddLangMateMemoryCache();

            // Assert
            var provider = result.BuildServiceProvider();
            Assert.NotNull(provider.GetService<IMemoryCache>());
            Assert.NotNull(provider.GetService<ICacheProvider>());
        }

        [Fact]
        public void AddLangMateMongoDb_RegistersMongoDbSettingsAndRepository()
        {
            // Arrange
            var services = new ServiceCollection();
            var configMock = new Mock<IConfiguration>();
            var sectionMock = new Mock<IConfigurationSection>();
            configMock.Setup(c => c.GetSection(nameof(MongoDbSettings))).Returns(sectionMock.Object);

            // Act
            var result = services.AddLangMateMongoDb(configMock.Object);

            // Assert
            var provider = result.BuildServiceProvider();
            Assert.NotNull(provider.GetService<IMongoDbSettings>());
            // Use a closed generic type for repository resolution
            Assert.NotNull(provider.GetService<IMongoRepository<object>>());
        }
    }
}