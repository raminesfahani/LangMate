using LangMate.Abstractions.Dto;
using Microsoft.Extensions.AI;

namespace LangMate.Abstractions.Contracts
{
    public interface IAICoreService
    {
        Task<AIResponse?> GenerateCompletionAsync(string prompt, ChatRole chatRole, bool newConversation = false, CancellationToken ct = default);
        IAsyncEnumerable<AIResponseChunk?> GenerateStreamingCompletionAsync(string prompt, ChatRole chatRole, bool newConversation = false, CancellationToken ct = default);
        List<string> GetAvailableModels();
        List<ChatMessage> GetChatHistory();
    }
}
