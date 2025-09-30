using LangMate.Abstractions.Abstracts.Documents;

namespace LangMate.Abstractions.Contracts
{
    /// <summary>
    /// Defines a contract for scraping available Ollama models from external sources (e.g., HTML or API).
    /// </summary>
    public interface IOllamaScraper
    {
        /// <summary>
        /// Scrapes and retrieves a list of available Ollama models from the configured source.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of <see cref="OllamaModel"/> objects.</returns>
        Task<List<OllamaModel>> ScrapeModelsAsync();
    }

}