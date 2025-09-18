using AutoMapper;
using LangMate.Abstractions.Enums;
using LangMate.Abstractions.Options;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangMate.Abstractions.Dto
{
    public class AIResponse : ChatResponse
    {
        /// <summary>
        /// The name of the provider (e.g. OpenAI, HuggingFace).
        /// </summary>
        public AIProviderEnum Provider { get; set; } = AIProviderEnum.LocalAI;

        /// <summary>
        /// The model used (e.g. gpt-3.5-turbo, llama2-13b).
        /// </summary>
        public string Model { get; set; } = string.Empty;

        /// <summary>Indicates if the result was returned from cache.</summary>
        public bool IsCached { get; set; } = false;
    }

    public static class AIResponseExtensions
    {
        public static AIResponse ToAIResponse(this ChatResponse chatResponse, IMapper mapper, AIOptions options)
        {
            var response = mapper.Map<AIResponse>(chatResponse);
            response.Model = options.Model;
            response.Provider = options.ProviderType;

            return response;
        }

        public static AIResponseChunk ToAIResponseChunk(this ChatResponseUpdate chatResponse, IMapper mapper, AIOptions options)
        {
            var response = mapper.Map<AIResponseChunk>(chatResponse);
            
            return response;
        }
    }

    public class LangMateMappingProfile : Profile
    {
        public LangMateMappingProfile()
        {
            CreateMap<ChatResponse, AIResponse>().ReverseMap();
            CreateMap<ChatResponseUpdate, AIResponseChunk>().ReverseMap();
        }
    }
}
