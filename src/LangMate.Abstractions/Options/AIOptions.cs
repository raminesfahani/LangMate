using LangMate.Abstractions.Enums;

namespace LangMate.Abstractions.Options
{
    public class AIOptions
    {
        /// <summary>
        /// Logical model name (e.g., "gpt-4", "llama2", "bloom", etc.).
        /// </summary>
        public string Model { get; set; } = "gemma3:4b";

        /// <summary>
        /// Temperature parameter for creativity (0–1).
        /// </summary>
        public double Temperature { get; set; } = 0.7;

        /// <summary>
        /// Maximum number of tokens to generate.
        /// </summary>
        public int MaxTokens { get; set; } = 1024;

        /// <summary>
        /// Language tag (e.g., "en", "fr", "de", "zh").
        /// </summary>
        public string Language { get; set; } = "en";

        /// <summary>
        /// Name of the provider: "OpenAI", "AzureOpenAI", "HuggingFace", "LocalAI".
        /// </summary>
        public AIProviderEnum Provider { get; set; } = AIProviderEnum.LocalAI;

        /// <summary>
        /// LocalAI: Path to the local model (optional).
        /// </summary>
        public string? LocalModelPath { get; set; }

        /// <summary>
        /// LocalAI: Model format (ggml, llama, etc.).
        /// </summary>
        public string? LocalModelFormat { get; set; }

        /// <summary>
        /// Custom settings (provider-specific options) as key-value pairs.
        /// </summary>
        public Dictionary<string, object> ExtraOptions { get; set; } = [];
    }
}
