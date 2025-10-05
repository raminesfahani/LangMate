namespace LangMate.Abstractions.Options
{
    public class OllamaOptions
    {
        public string ModelLibraryUrl { get; set; } = "https://ollama.com/library";

        /// <summary>
        /// Temperature parameter for creativity (0–1).
        /// </summary>
        public double Temperature { get; set; } = 0.7;

        /// <summary>
        /// Maximum number of tokens to generate.
        /// </summary>
        public int MaxTokens { get; set; } = 1024;

        /// <summary>
        /// API key for authentication (if required).
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Custom settings (provider-specific options) as key-value pairs.
        /// </summary>
        public Dictionary<string, object> ExtraOptions { get; set; } = [];
    }
}
