using HtmlAgilityPack;
using LangMate.Abstractions.Abstracts.Documents;
using LangMate.Abstractions.Contracts;
using LangMate.Abstractions.Options;
using Microsoft.Extensions.Options;

namespace LangMate.Core.Ollama
{
    public class OllamaScraper(IOptions<OllamaOptions> options) : IOllamaScraper
    {
        private readonly string Url = options.Value.ModelLibraryUrl;
        private readonly HttpClient _httpClient = new();

        public async Task<List<OllamaModel>> ScrapeModelsAsync()
        {
            var html = await _httpClient.GetStringAsync(Url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var modelList = new List<OllamaModel>();

            // Based on current structure, each model is in a <li> with a title and a paragraph
            var items = doc.DocumentNode.SelectNodes("//main//ul/li");

            if (items == null)
            {
                Console.WriteLine("⚠ No model items found. Site structure may have changed.");
                return modelList;
            }

            foreach (var item in items)
            {
                try
                {
                    var nameNode = item.SelectSingleNode(".//h2");
                    var descNode = item.SelectSingleNode(".//p");

                    if (nameNode == null) continue;

                    modelList.Add(new OllamaModel
                    {
                        Name = nameNode.InnerText.Trim(),
                        Description = descNode?.InnerText.Trim() ?? ""
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return modelList;
        }
    }

}
