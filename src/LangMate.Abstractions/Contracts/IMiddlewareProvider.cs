using LangMate.Abstractions.Dto;
using LangMate.Abstractions.Options;
using Microsoft.Extensions.AI;

namespace LangMate.Abstractions.Contracts
{
    public interface IMiddlewareProvider
    {
        Task<AIResponse?> InvokeAsync(string prompt, AIOptions options, ChatRole chatRole, IAIClient client, bool newConversation = false, CancellationToken ct = default);
    }
}
