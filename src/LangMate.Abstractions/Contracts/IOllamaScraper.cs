using LangMate.Abstractions.Abstracts.Documents;

namespace LangMate.Abstractions.Contracts
{
    public interface IOllamaScraper
    {
        Task<List<OllamaModel>> ScrapeModelsAsync();
    }
}