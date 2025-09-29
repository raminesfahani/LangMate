using LangMate.Abstractions.Abstracts.Documents;
using LangMate.Abstractions.Abstracts.Persistence;
using LangMate.Abstractions.Contracts;
using LangMate.Abstractions.Options;
using Microsoft.Extensions.Options;
using Ollama;
using System.IO;
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
            await CheckRequestModelValidation(request);

            var conversation = new ConversationDocument
            {
                Title = TruncateMessage(request.Messages.FirstOrDefault(x => x.Role == MessageRole.User)?.Content ?? request.Messages[0].Content, 40),
                Messages = [.. request.Messages],
                Model = request.Model,
            };

            await _conversationRepo.InsertOneAsync(conversation);

            var stream = _client.Chat.GenerateChatCompletionAsync(request, ct);

            // Wrap the stream to detect completion
            var response = "";
            async IAsyncEnumerable<GenerateChatCompletionResponse> WrappedStream(IAsyncEnumerable<GenerateChatCompletionResponse> originalStream)
            {
                await foreach (var item in originalStream.WithCancellation(ct))
                {
                    response += item?.Message.Content;
                    if (item == null) continue;

                    yield return item;
                }

                // This runs after the iteration is finished
                await OnStreamCompletedAsync(conversation.ConversationId, response);
            }

            return (conversation.Id.ToString(), WrappedStream(stream));
        }

        // Example callback
        private async Task OnStreamCompletedAsync(string conversationId, string content)
        {
            await AddMessageToConversation(conversationId, new Message()
            {
                Role = MessageRole.Assistant,
                Content = content
            });
        }

        private async Task CheckRequestModelValidation(GenerateChatCompletionRequest request)
        {
            if (request.Messages == null || request.Messages.Any() == false)
            {
                throw new ArgumentException("The request must contain at least one message to start a new conversation.");
            }

            var localModels = await GetModelsListAsync();
            if (string.IsNullOrWhiteSpace(request.Model) || localModels.Any(x => x.Name == request.Model) == false)
            {
                throw new ArgumentException($"The model '{request.Model}' is not available. Please choose from the available models.");
            }
        }

        public async Task<IAsyncEnumerable<GenerateChatCompletionResponse>> GenerateChatCompletionAsync(string conversationId,
                                                                                      GenerateChatCompletionRequest request,
                                                                                      CancellationToken ct = default)
        {
            await CheckRequestModelValidation(request);

            var conversation = await _conversationRepo.FindByIdAsync(conversationId) ?? throw new KeyNotFoundException($"Conversation with ID {conversationId} not found.");
            conversation.UpdatedAt = DateTime.UtcNow;
            conversation.Messages.AddRange(request.Messages);
            await _conversationRepo.ReplaceOneAsync(conversation);

            request.Messages = conversation.Messages;
            var stream = _client.Chat.GenerateChatCompletionAsync(request, ct);

            // Wrap the stream to detect completion
            var response = "";
            async IAsyncEnumerable<GenerateChatCompletionResponse> WrappedStream(IAsyncEnumerable<GenerateChatCompletionResponse> originalStream)
            {
                await foreach (var item in originalStream.WithCancellation(ct))
                {
                    response += item?.Message.Content;
                    if (item == null) continue;

                    yield return item;
                }

                // This runs after the iteration is finished
                await OnStreamCompletedAsync(conversation.ConversationId, response);
            }

            return WrappedStream(stream);
        }

        public async Task AddMessageToConversation(string conversationId, Message message)
        {
            var conversation = await _conversationRepo.FindByIdAsync(conversationId) ?? throw new KeyNotFoundException($"Conversation with ID {conversationId} not found.");

            conversation.Messages.Add(message);
            conversation.UpdatedAt = DateTime.UtcNow;
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

        public IList<ConversationDocument> GetAllConversations(string search = "")
        {
            return [.. _conversationRepo.FilterBy(x => string.IsNullOrEmpty(search) || x.Title.Contains(search) || x.Messages.Contains(search)).OrderByDescending(x => x.UpdatedAt)];
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
            List<OllamaModel>? models = GetOllamaModelsCache(lifetime: TimeSpan.FromHours(2));

            if (models != null && models.Count != 0)
                return models;

            models = await UpdateOllamaModelsCacheAsync();
            return models;
        }

        private List<OllamaModel> GetOllamaModelsCache(TimeSpan lifetime)
        {
            return [.. _ollamaModelsRepo.FilterBy(x => x.CreatedAt.Add(lifetime) >= DateTime.UtcNow)];
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
