using LangMate.Abstractions.Abstracts.Documents;
using LangMate.Abstractions.Abstracts.Persistence;
using LangMate.Abstractions.Contracts;
using LangMate.Abstractions.Options;
using Microsoft.Extensions.Options;
using Ollama;
using System.Reflection;

namespace LangMate.Core.Providers
{
    public class OllamaFactoryProvider(IOptions<OllamaOptions> options,
                                        IOllamaApiClient client,
                                        ICacheProvider cache,
                                        IOllamaScraper scraper,
                                        IMongoRepository<OllamaModel> ollamaModelsRepo,
                                       IMongoRepository<ConversationDocument> conversationRepo) : IOllamaFactoryProvider
    {
        private readonly IOptions<OllamaOptions> _options = options;
        private readonly IOllamaApiClient _client = client;
        private readonly ICacheProvider _cache = cache;
        private readonly IOllamaScraper _scraper = scraper;
        private readonly IMongoRepository<OllamaModel> _ollamaModelsRepo = ollamaModelsRepo;
        private readonly IMongoRepository<ConversationDocument> _conversationRepo = conversationRepo;

        public IOllamaApiClient Client => _client;

        public async Task<ModelsResponse> GetAvailableModelsAsync(CancellationToken ct = default)
        {
            return await _client.Models.ListModelsAsync(ct);
        }

        public async Task<(string conversationId, IAsyncEnumerable<GenerateChatCompletionResponse> response)>
            StartNewChatCompletionAsync(GenerateChatCompletionRequest request, CancellationToken ct = default)
        {
            if (request.Messages == null || request.Messages.Any() == false)
            {
                throw new ArgumentException("The request must contain at least one message to start a new conversation.");
            }

            var conversation = new ConversationDocument
            {
                Title = TruncateMessage(request.Messages.FirstOrDefault(x => x.Role == MessageRole.User)?.Content ?? request.Messages[0].Content, 20),
                Messages = [.. request.Messages]
            };

            await _conversationRepo.InsertOneAsync(conversation);

            return (conversationId: conversation.Id.ToString(), _client.Chat.GenerateChatCompletionAsync(request, ct));
        }

        public async Task<IAsyncEnumerable<GenerateChatCompletionResponse>> GenerateChatCompletionAsync(string conversationId,
                                                                                      GenerateChatCompletionRequest request,
                                                                                      CancellationToken ct = default)
        {
            var conversation = await _conversationRepo.FindByIdAsync(conversationId) ?? throw new KeyNotFoundException($"Conversation with ID {conversationId} not found.");
            conversation.Messages.AddRange(request.Messages);
            await _conversationRepo.ReplaceOneAsync(conversation);

            request.Messages = conversation.Messages;

            return _client.Chat.GenerateChatCompletionAsync(request, ct);
        }

        public async Task AddMessageToConversation(string conversationId, Message message)
        {
            var conversation = await _conversationRepo.FindByIdAsync(conversationId) ?? throw new KeyNotFoundException($"Conversation with ID {conversationId} not found.");
            conversation.Messages.Add(message);
            await _conversationRepo.ReplaceOneAsync(conversation);
        }

        public async Task DeleteConversationAsync(string conversationId)
        {
            var conversation = await _conversationRepo.FindByIdAsync(conversationId) ?? throw new KeyNotFoundException($"Conversation with ID {conversationId} not found.");
            await _conversationRepo.DeleteByIdAsync(conversation.Id.ToString());
        }

        public async Task<ConversationDocument> GetConversationAsync(string conversationId)
        {
            var conversation = await _conversationRepo.FindByIdAsync(conversationId) ?? throw new KeyNotFoundException($"Conversation with ID {conversationId} not found.");
            return conversation;
        }

        public IList<ConversationDocument> GetAllConversations()
        {
            return [.. _conversationRepo.FilterBy(x => x.Messages.Any()).OrderByDescending(x => x.CreatedAt)];
        }

        public IAsyncEnumerable<PullModelResponse> PullModelAsync(string model, CancellationToken ct = default)
        {
            return _client.Models.PullModelAsync(new PullModelRequest
            {
                Model = model,
                Stream = true,
            }, ct);
        }

        public async Task<List<OllamaModel>> GetModelsListAsync()
        {
            List<OllamaModel>? models = GetOllamaModelsCache();

            if (models != null && models.Count != 0)
                return models;

            models = await UpdateOllamaModelsCacheAsync();
            return models;
        }

        private List<OllamaModel> GetOllamaModelsCache()
        {
            return [.. _ollamaModelsRepo.FilterBy(x => x.CreatedAt.AddHours(2) >= DateTime.UtcNow)];
        }

        private async Task<List<OllamaModel>> UpdateOllamaModelsCacheAsync()
        {
            _ollamaModelsRepo.DeleteMany(x => true);

            var models = await _scraper.ScrapeModelsAsync();
            await _ollamaModelsRepo.InsertManyAsync(models);

            return models;
        }

        private static string TruncateMessage(string message, int maxLength)
        {
            if (message.Length <= maxLength)
                return message;
            return message[..maxLength];
        }
    }
}
