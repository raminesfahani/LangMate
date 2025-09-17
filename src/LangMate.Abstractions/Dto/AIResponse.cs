using LangMate.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangMate.Abstractions.Dto
{
    public class AIResponse
    {
        /// <summary>
        /// The final generated text response.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// The name of the provider (e.g. OpenAI, HuggingFace).
        /// </summary>
        public AIProviderEnum Provider { get; set; } = AIProviderEnum.LocalAI;

        /// <summary>
        /// The model used (e.g. gpt-3.5-turbo, llama2-13b).
        /// </summary>
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Optional reason for finishing generation (e.g. stop, length).
        /// </summary>
        public string FinishReason { get; set; } = string.Empty;

        /// <summary>
        /// Optional total tokens used (if available).
        /// </summary>
        public int? TotalTokens { get; set; }

        /// <summary>
        /// Optional raw response (serialized JSON or raw body) for debugging or inspection.
        /// </summary>
        public string RawResponse { get; set; } = string.Empty;

        /// <summary>Indicates if the result was returned from cache.</summary>
        public bool IsCached { get; set; } = false;
    }
}
