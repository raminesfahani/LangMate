using LangMate.Abstractions.Contracts;
using LangMate.Abstractions.Dto;
using LangMate.Abstractions.Options;
using LangMate.Cache;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace LangMate.Middleware
{
    public class MiddlewareProvider(ICacheProvider cache, ILogger<MiddlewareProvider> logger) : IMiddlewareProvider
    {
        private readonly ICacheProvider _cache = cache;
        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(2);
        private readonly ILogger<MiddlewareProvider> _logger = logger;

        public async Task<AIResponse?> InvokeAsync(string prompt, AIOptions options, ChatRole chatRole, IAIClient client, bool newConversation = false, CancellationToken ct = default)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("🚀 AI Request Started | Provider: {Provider} | Model: {Model} | Lang: {Language} | Prompt: {PromptPreview}",
                options.ProviderType,
                options.Model,
                options.Language,
                Truncate(prompt, 100)
            );

            try
            {
                var key = CacheKeyHelper.GenerateKey(options.Model, options.Language, prompt);

                if (_cache.TryGet<AIResponse>(key, out var cachedResponse))
                {
                    return cachedResponse;
                }

                var response = await client.GenerateCompletionAsync(prompt, options, chatRole, newConversation, ct);

                stopwatch.Stop();

                _logger.LogInformation("✅ AI Response Completed | Tokens: {Tokens} | Cached: {IsCached} | Time: {Elapsed}ms | Output: {ResponsePreview}",
                        response?.Usage?.TotalTokenCount ?? 0,
                        response?.IsCached,
                        stopwatch.ElapsedMilliseconds,
                        Truncate(response?.Text ?? "", 100)
                    );

                _cache.Set(key, response, _ttl);
                return response;
            }
            catch (Exception ex)
            {

                stopwatch.Stop();

                _logger.LogError(ex, "❌ AI Request Failed | Provider: {Provider} | Model: {Model} | Time: {Elapsed}ms",
                    options.ProviderType,
                    options.Model,
                    stopwatch.ElapsedMilliseconds
                );

                throw;
            }
        }

        private static string Truncate(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input.Length <= maxLength ? input : string.Concat(input.AsSpan(0, maxLength), "...");
        }
    }
}
