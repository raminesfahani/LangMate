using LangMate.Core.Ollama;
using LangMate.Abstractions.Options;
using LangMate.Abstractions.Contracts;
using LangMate.Abstractions.Abstracts.Persistence;
using LangMate.Abstractions.Abstracts.Documents;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Ollama;

namespace LangMate.Core.Tests
{
    public class OllamaFactoryTests
    {
        private static OllamaFactory CreateFactory(
            IOllamaApiClient? client = null,
            ICacheProvider? cache = null,
            IOllamaScraper? scraper = null,
            IMongoRepository<OllamaModel>? modelsRepo = null,
            IMongoRepository<ConversationDocument>? convRepo = null)
        {
            var options = Options.Create(new OllamaOptions());
            return new OllamaFactory(
                options,
                client ?? new Mock<IOllamaApiClient>().Object,
                cache ?? new Mock<ICacheProvider>().Object,
                scraper ?? new Mock<IOllamaScraper>().Object,
                modelsRepo ?? new Mock<IMongoRepository<OllamaModel>>().Object,
                convRepo ?? new Mock<IMongoRepository<ConversationDocument>>().Object
            );
        }

        [Fact]
        public void Constructor_SetsClientProperty()
        {
            var clientMock = new Mock<IOllamaApiClient>();
            var factory = CreateFactory(client: clientMock.Object);

            Assert.Equal(clientMock.Object, factory.Client);
        }


        [Fact]
        public async Task CheckRequestModelValidation_ThrowsIfNoMessages()
        {
            var factory = CreateFactory();
            var request = new GenerateChatCompletionRequest { Messages = [], Model = "test" };

            var method = factory.GetType().GetMethod("CheckRequestModelValidation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                (Task)method.Invoke(factory, [request])
            );
        }
    }
}