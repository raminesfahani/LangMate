namespace LangMate.Abstractions.Options
{
    public class OllamaOptions
    {
        /// <summary>
        /// Logical model name (e.g., "gpt-4", "llama2", "bloom", etc.).
        /// </summary>
        public string Model { get; set; } = "llama3.1";
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
        /// Language tag (e.g., "en", "fr", "de", "zh").
        /// </summary>
        public string Language { get; set; } = "en";

        /// <summary>
        /// The endpoint URL for the AI service (if applicable).
        /// </summary>
        public string Endpoint { get; set; } = "http://localhost:11434/";

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
