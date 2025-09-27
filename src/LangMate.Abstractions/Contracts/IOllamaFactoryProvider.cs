
using LangMate.Abstractions.Abstracts.Documents;
using Ollama;

namespace LangMate.Abstractions.Contracts
{
    public interface IOllamaFactoryProvider
    {
        IOllamaApiClient Client { get; }

        Task AddMessageToConversation(string conversationId, Message message);
        Task DeleteConversationAsync(string conversationId);
        Task<IAsyncEnumerable<GenerateChatCompletionResponse>> GenerateChatCompletionAsync(string conversationId, GenerateChatCompletionRequest request, CancellationToken ct = default);
        IList<ConversationDocument> GetAllConversations(string search = "");
        Task<ModelsResponse> GetAvailableModelsAsync(CancellationToken ct = default);
        Task<ConversationDocument> GetConversationAsync(string conversationId);
        Task<List<OllamaModel>> GetModelsListAsync();
        IAsyncEnumerable<PullModelResponse> PullModelAsync(string model, CancellationToken ct = default);
        Task<(string conversationId, IAsyncEnumerable<GenerateChatCompletionResponse> response)> StartNewChatCompletionAsync(GenerateChatCompletionRequest request, CancellationToken ct = default);
    }
}
