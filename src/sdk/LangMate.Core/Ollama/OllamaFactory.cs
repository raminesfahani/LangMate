using LangMate.Abstractions.Abstracts.Documents;
using LangMate.Abstractions.Abstracts.Persistence;
using LangMate.Abstractions.Contracts;
using LangMate.Abstractions.Options;
using Microsoft.Extensions.Options;
using Ollama;
using System.IO;
using System.Reflection;

namespace LangMate.Core.Ollama
{
    /// <summary>
    /// Provides the core implementation of <see cref="IOllamaFactory"/> for managing Ollama chat operations,
    /// model interactions, and MongoDB-based persistence of chat history.
    /// </summary>
    public class OllamaFactory(
        IOptions<OllamaOptions> options,
        IOllamaApiClient client,
        ICacheProvider cache,
        IOllamaScraper scraper,
        IMongoRepository<OllamaModel> ollamaModelsRepo,
        IMongoRepository<ConversationDocument> conversationRepo
    ) : IOllamaFactory
    {
        private readonly IOptions<OllamaOptions> _options = options;
        private readonly IOllamaApiClient _client = client;
        private readonly ICacheProvider _cache = cache;
        private readonly IOllamaScraper _scraper = scraper;
        private readonly IMongoRepository<OllamaModel> _ollamaModelsRepo = ollamaModelsRepo;
        private readonly IMongoRepository<ConversationDocument> _conversationRepo = conversationRepo;

        /// <inheritdoc />
        public IOllamaApiClient Client => _client;

        /// <inheritdoc />
        public async Task<ModelsResponse> GetAvailableModelsAsync(CancellationToken ct = default)
        {
            return await _client.Models.ListModelsAsync(ct);
        }

        /// <inheritdoc />
        public async Task<(string conversationId, IAsyncEnumerable<GenerateChatCompletionResponse> response)> StartNewChatCompletionAsync(
            GenerateChatCompletionRequest request, CancellationToken ct = default)
        {
            await CheckRequestModelValidation(request);

            var conversation = new ConversationDocument
            {
                Title = TruncateMessage(request.Messages.FirstOrDefault(x => x.Role == MessageRole.User)?.Content ?? request.Messages[0].Content, 40),
                Messages = [.. request.Messages],
                Model = request.Model,
            };

            await _conversationRepo.InsertOneAsync(conversation);
            var stream = _client.Chat.GenerateChatCompletionAsync(request, ct);

            var response = "";
            async IAsyncEnumerable<GenerateChatCompletionResponse> WrappedStream(IAsyncEnumerable<GenerateChatCompletionResponse> originalStream)
            {
                await foreach (var item in originalStream.WithCancellation(ct))
                {
                    response += item?.Message.Content;
                    if (item == null) continue;

                    yield return item;
                }

                await OnStreamCompletedAsync(conversation.ConversationId, response);
            }

            return (conversation.Id.ToString(), WrappedStream(stream));
        }

        /// <summary>
        /// Callback when a streaming chat completes; adds assistant response to conversation history.
        /// </summary>
        private async Task OnStreamCompletedAsync(string conversationId, string content)
        {
            await AddMessageToConversation(conversationId, new Message
            {
                Role = MessageRole.Assistant,
                Content = content
            });
        }

        /// <summary>
        /// Validates the incoming chat request including required messages and model existence.
        /// </summary>
        private async Task CheckRequestModelValidation(GenerateChatCompletionRequest request)
        {
            if (request.Messages == null || request.Messages.Any() == false)
                throw new ArgumentException("The request must contain at least one message.");

            var localModels = await GetModelsListAsync();
            if (string.IsNullOrWhiteSpace(request.Model) || localModels.Any(x => x.Name == request.Model) == false)
                throw new ArgumentException($"The model '{request.Model}' is not available.");
        }

        /// <inheritdoc />
        public async Task<IAsyncEnumerable<GenerateChatCompletionResponse>> GenerateChatCompletionAsync(
            string conversationId, GenerateChatCompletionRequest request, CancellationToken ct = default)
        {
            await CheckRequestModelValidation(request);

            var conversation = await _conversationRepo.FindByIdAsync(conversationId)
                ?? throw new KeyNotFoundException($"Conversation with ID {conversationId} not found.");

            conversation.UpdatedAt = DateTime.UtcNow;
            conversation.Messages.AddRange(request.Messages);
            await _conversationRepo.ReplaceOneAsync(conversation);

            request.Messages = conversation.Messages;
            var stream = _client.Chat.GenerateChatCompletionAsync(request, ct);

            var response = "";
            async IAsyncEnumerable<GenerateChatCompletionResponse> WrappedStream(IAsyncEnumerable<GenerateChatCompletionResponse> originalStream)
            {
                await foreach (var item in originalStream.WithCancellation(ct))
                {
                    response += item?.Message.Content;
                    if (item == null) continue;
                    yield return item;
                }

                await OnStreamCompletedAsync(conversation.ConversationId, response);
            }

            return WrappedStream(stream);
        }

        /// <inheritdoc />
        public async Task AddMessageToConversation(string conversationId, Message message)
        {
            var conversation = await _conversationRepo.FindByIdAsync(conversationId)
                ?? throw new KeyNotFoundException($"Conversation with ID {conversationId} not found.");

            conversation.Messages.Add(message);
            conversation.UpdatedAt = DateTime.UtcNow;
            await _conversationRepo.ReplaceOneAsync(conversation);
        }

        /// <inheritdoc />
        public async Task DeleteConversationAsync(string conversationId)
        {
            var conversation = await _conversationRepo.FindByIdAsync(conversationId)
                ?? throw new KeyNotFoundException($"Conversation with ID {conversationId} not found.");

            await _conversationRepo.DeleteByIdAsync(conversation.Id.ToString());
        }

        /// <inheritdoc />
        public async Task<ConversationDocument> GetConversationAsync(string conversationId)
        {
            return await _conversationRepo.FindByIdAsync(conversationId)
                ?? throw new KeyNotFoundException($"Conversation with ID {conversationId} not found.");
        }

        /// <inheritdoc />
        public IList<ConversationDocument> GetAllConversations(string search = "")
        {
            return [.. _conversationRepo
            .FilterBy(x => string.IsNullOrEmpty(search) || x.Title.Contains(search) || x.Messages.Contains(search))
            .OrderByDescending(x => x.UpdatedAt)];
        }

        /// <inheritdoc />
        public IAsyncEnumerable<PullModelResponse> PullModelAsync(string model, CancellationToken ct = default)
        {
            return _client.Models.PullModelAsync(new PullModelRequest
            {
                Model = model,
                Stream = true,
            }, ct);
        }

        /// <inheritdoc />
        public async Task<List<OllamaModel>> GetModelsListAsync()
        {
            var models = GetOllamaModelsCache(TimeSpan.FromHours(2));
            return models.Count > 0 ? models : await UpdateOllamaModelsCacheAsync();
        }

        /// <summary>
        /// Returns locally cached models that are still valid based on the cache lifetime.
        /// </summary>
        private List<OllamaModel> GetOllamaModelsCache(TimeSpan lifetime)
        {
            return [.. _ollamaModelsRepo.FilterBy(x => x.CreatedAt.Add(lifetime) >= DateTime.UtcNow)];
        }

        /// <summary>
        /// Updates the local model cache by scraping the Ollama model list and persisting to MongoDB.
        /// </summary>
        private async Task<List<OllamaModel>> UpdateOllamaModelsCacheAsync()
        {
            _ollamaModelsRepo.DeleteMany(x => true);
            var models = await _scraper.ScrapeModelsAsync();
            await _ollamaModelsRepo.InsertManyAsync(models);
            return models;
        }

        /// <summary>
        /// Truncates a message to a maximum length, preserving initial characters.
        /// </summary>
        private static string TruncateMessage(string message, int maxLength)
        {
            return message.Length <= maxLength ? message : message[..maxLength];
        }
    }

}
