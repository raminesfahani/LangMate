using AutoMapper;
using LangMate.Abstractions.Contracts;
using LangMate.Abstractions.Dto;
using LangMate.Abstractions.Enums;
using LangMate.Abstractions.Options;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Runtime.CompilerServices;

namespace LangMate.Providers.LocalAI
{
    public class LocalAIClient(AIOptions options, IMapper mapper) : IAIClient
    {
        protected IChatClient? _chatClient;
        private static readonly List<ChatMessage> _chatHistory = [];
        private readonly AIOptions _options = options;
        private readonly IMapper _mapper = mapper;
        private List<string> _availableModels = [];

        public async Task Initialize()
        {
            if (_options.LocalProviderType == LocalAIEnum.Ollama)
            {
                var ollamaApiClient = new OllamaApiClient(new Uri(_options.LocalModelEndpoint), _options.LocalModel);
                _chatClient = ollamaApiClient;

                _availableModels = [.. (await ollamaApiClient.ListLocalModelsAsync()).Select(x => x.Name)];

                // Pulling a model and reporting progress
                await foreach (var response in ollamaApiClient.PullModelAsync(_options.LocalModel))
                {
                    Console.WriteLine($"{response?.Status}. Progress: {response?.Completed}/{response?.Total}");
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public List<string> GetAvailableModels()
        {
            if (_chatClient == null)
                throw new NotImplementedException();
            return _availableModels;
        }

        public async Task<AIResponse?> GenerateCompletionAsync(string prompt, AIOptions options, ChatRole chatRole, bool newConversation = false, CancellationToken ct = default)
        {
            if (_chatClient == null)
                throw new NotImplementedException();

            if (newConversation == true) _chatHistory.Clear();

            var message = new ChatMessage(ChatRole.User, prompt);
            _chatHistory.Add(message);

            var response = await _chatClient.GetResponseAsync(_chatHistory, new ChatOptions()
            {
                MaxOutputTokens = options.MaxTokens,
                Temperature = (float)options.Temperature,
            }, cancellationToken: ct);

            if (response == null) return null;

            _chatHistory.Add(new ChatMessage(ChatRole.Assistant, response.Text));

            return response.ToAIResponse(_mapper, options);
        }

        public async IAsyncEnumerable<AIResponseChunk?> GenerateStreamCompletionAsync(string prompt, AIOptions options, ChatRole chatRole, bool newConversation = false, [EnumeratorCancellation] CancellationToken ct = default)
        {
            if (_chatClient == null)
                throw new NotImplementedException();

            if (newConversation == true) _chatHistory.Clear();

            var message = new ChatMessage(ChatRole.User, prompt);
            _chatHistory.Add(message);

            var response = "";
            await foreach (ChatResponseUpdate item in
                _chatClient.GetStreamingResponseAsync(_chatHistory, new ChatOptions()
                {
                    MaxOutputTokens = options.MaxTokens,
                    Temperature = (float)options.Temperature,
                }, cancellationToken: ct))
            {
                response += item.Text;
                yield return item.ToAIResponseChunk(_mapper, options);
            }
            _chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
        }

        public List<ChatMessage> GetChatHistory() => _chatHistory;
    }
}
