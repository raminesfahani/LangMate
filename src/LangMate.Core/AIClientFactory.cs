using AutoMapper;
using LangMate.Abstractions.Contracts;
using LangMate.Abstractions.Enums;
using LangMate.Abstractions.Options;
using LangMate.Providers.LocalAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangMate.Core
{
    public class AIClientFactory
    {
        internal static IAIClient CreateClient(AIOptions options, IMapper mapper)
        {
            if (options.ProviderType == AIProviderEnum.LocalAI)
            {
                var client = new LocalAIClient(options, mapper);
                client.Initialize().GetAwaiter().GetResult();
                return client;
            }
            //else if (options.ProviderType == AIProviderEnum.OpenAI)
            //{
            //    return new OpenAIClient(options.ApiKey);
            //}
            //else if (options.ProviderType == AIProviderEnum.AzureOpenAI)
            //{
            //    return new AzureOpenAIClient(options.ApiKey, options.Endpoint, options.DeploymentName);
            //}
            else
            {
                throw new NotSupportedException($"The AI provider type '{options.ProviderType}' is not supported.");
            }
        }
    }
}
