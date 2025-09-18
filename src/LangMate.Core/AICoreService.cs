using AutoMapper;
using LangMate.Abstractions.Contracts;
using LangMate.Abstractions.Dto;
using LangMate.Abstractions.Options;
using LangMate.Middleware;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangMate.Core
{
    public class AICoreService(IMiddlewareProvider middlewareProvider, IOptions<AIOptions> options, IAIClient client) : IAICoreService
    {
        private readonly IMiddlewareProvider _middlewareProvider = middlewareProvider;
        private readonly IOptions<AIOptions> _options = options;
        private readonly IAIClient _client = client;

        /// <summary>
        /// Get list of available models from the AI provider.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAvailableModels()
        {
            return _client.GetAvailableModels();
        }

        /// <summary>
        /// Get the list of chat messages
        /// </summary>
        /// <returns></returns>
        public List<ChatMessage> GetChatHistory()
        {
            return _client.GetChatHistory();
        }

        /// <summary>
        /// Generates a full AI response based on the prompt and options provided.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="chatRole"></param>
        /// <param name="newConversation"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<AIResponse?> GenerateCompletionAsync(string prompt, ChatRole chatRole, bool newConversation = false, CancellationToken ct = default)
        {
            var response = await _middlewareProvider.InvokeAsync(prompt, _options.Value, chatRole, _client, newConversation, ct);
            return response;
        }

        /// <summary>
        /// Generates a streaming AI response based on the prompt and options provided.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="chatRole"></param>
        /// <param name="newConversation"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public IAsyncEnumerable<AIResponseChunk?> GenerateStreamingCompletionAsync(string prompt, ChatRole chatRole, bool newConversation = false, CancellationToken ct = default)
        {
            return _client.GenerateStreamCompletionAsync(prompt, _options.Value, chatRole, newConversation, ct);
        }
    }
}
