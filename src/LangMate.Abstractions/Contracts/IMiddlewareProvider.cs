using LangMate.Abstractions.Dto;
using LangMate.Abstractions.Options;

namespace LangMate.Abstractions.Contracts
{
    public interface IMiddlewareProvider
    {
        Task<AIResponse> InvokeAsync(string prompt, AIOptions options, IAIClient next, CancellationToken ct = default);
    }
}
