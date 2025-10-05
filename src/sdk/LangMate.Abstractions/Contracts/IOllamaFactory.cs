
using LangMate.Abstractions.Abstracts.Documents;
using Ollama;

namespace LangMate.Abstractions.Contracts
{
    /// <summary>
    /// Defines a factory interface for managing interactions with the Ollama AI client,
    /// including model handling, conversation persistence, and streaming chat completions.
    /// </summary>
    public interface IOllamaFactory
    {
        /// <summary>
        /// Gets the underlying Ollama API client instance.
        /// </summary>
        IOllamaApiClient Client { get; }

        /// <summary>
        /// Adds a new message to the specified conversation.
        /// </summary>
        /// <param name="conversationId">The unique identifier of the conversation.</param>
        /// <param name="message">The message object to be added.</param>
        Task AddMessageToConversation(string conversationId, Message message);

        /// <summary>
        /// Deletes a conversation and its associated messages.
        /// </summary>
        /// <param name="conversationId">The ID of the conversation to delete.</param>
        Task DeleteConversationAsync(string conversationId);

        /// <summary>
        /// Generates a streaming chat completion for an existing conversation.
        /// </summary>
        /// <param name="conversationId">The ID of the conversation to continue.</param>
        /// <param name="request">The request object containing model, messages, and stream options.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>A stream of chat completion responses as an asynchronous enumerable.</returns>
        Task<IAsyncEnumerable<GenerateChatCompletionResponse>> GenerateChatCompletionAsync(string conversationId, GenerateChatCompletionRequest request, CancellationToken ct = default);

        /// <summary>
        /// Gets all stored conversations, optionally filtered by a search string.
        /// </summary>
        /// <param name="search">Optional search term to filter conversations by title or message content.</param>
        /// <returns>A list of conversation metadata documents.</returns>
        IList<ConversationDocument> GetAllConversations(string search = "");

        /// <summary>
        /// Retrieves all available local or remote Ollama models.
        /// </summary>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>A list of available model metadata.</returns>
        Task<ModelsResponse> GetInstalledModelsAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the full conversation document, including message history, by conversation ID.
        /// </summary>
        /// <param name="conversationId">The ID of the conversation to retrieve.</param>
        /// <returns>The full conversation document, or null if not found.</returns>
        Task<ConversationDocument> GetConversationAsync(string conversationId);

        /// <summary>
        /// Retrieves a list of all locally available Ollama models.
        /// </summary>
        /// <returns>A list of Ollama model metadata.</returns>
        Task<List<OllamaModel>> GetAvailableModelsListAsync();

        /// <summary>
        /// Streams the model download process from Ollama.
        /// </summary>
        /// <param name="model">The name or tag of the model to pull.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>An asynchronous stream of pull status updates.</returns>
        IAsyncEnumerable<PullModelResponse> PullModelAsync(string model, CancellationToken ct = default);

        /// <summary>
        /// Starts a new conversation and initiates a streaming chat completion.
        /// </summary>
        /// <param name="request">The chat completion request containing messages, model, and stream settings.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>A tuple containing the new conversation ID and a stream of completion responses.</returns>
        Task<(string conversationId, IAsyncEnumerable<GenerateChatCompletionResponse> response)> StartNewChatCompletionAsync(GenerateChatCompletionRequest request, CancellationToken ct = default);
    }
}
