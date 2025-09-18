using LangMate.Abstractions.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OllamaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangMate.Providers.LocalAI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddLocalAIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAIServices(configuration);
        }

        private static void AddAIServices(this IServiceCollection services, IConfiguration configuration)
        {
            var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
            var aiOptions = configuration.GetValue<AIOptions>(nameof(AIOptions));
            aiOptions ??= new()
            {
                
            };

            string? ollamaEndpoint = aiOptions.LocalModelEndpoint;
            if (!string.IsNullOrWhiteSpace(ollamaEndpoint))
            {
                services.AddChatClient(new OllamaApiClient(ollamaEndpoint, aiOptions.LocalModel ?? "llama3.1"))
                    .UseFunctionInvocation()
                    .UseOpenTelemetry(configure: t => t.EnableSensitiveData = true)
                    .UseLogging(loggerFactory)
                    .Build();
            }
        }
    }
}
