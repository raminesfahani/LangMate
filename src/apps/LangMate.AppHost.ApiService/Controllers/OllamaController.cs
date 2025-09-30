using LangMate.Abstractions.Contracts;
using Microsoft.AspNetCore.Mvc;
using Ollama;
using System.ComponentModel.DataAnnotations;

namespace LangMate.AppHost.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OllamaController(ILogger<OllamaController> logger, IOllamaFactory ollamaFactoryProvider) : ControllerBase
    {
        private readonly ILogger<OllamaController> _logger = logger;
        private readonly IOllamaFactory _ollamaFactoryProvider = ollamaFactoryProvider;

        [HttpGet("models")]
        public async Task<IActionResult> GetModelListAsync(string term = "")
        {
            var models = await _ollamaFactoryProvider.GetModelsListAsync();

            return Ok(models.Where(x => x.Name.Contains(term) || x.Description.Contains(term)));
        }

        [HttpGet("models/installed")]
        public async Task<IActionResult> GetLocalModels()
        {
            var models = await _ollamaFactoryProvider.GetAvailableModelsAsync();
            return Ok(models);
        }

        [HttpPut("models/pull/{model}")]
        public async Task<IActionResult> PullModelAsync([Required] string model, CancellationToken cancellationToken)
        {
            var response = _ollamaFactoryProvider.PullModelAsync(model, cancellationToken);
            PullModelResponse? first = null;
            
            await foreach (var progress in response)
            {
                first ??= progress;

                if (first == null || progress.Completed == null || progress.Total == null)
                    continue;

                double completedMB = progress.Completed.Value / 1_000_000.0;
                double totalMB = progress.Total.Value / 1_000_000.0;
                double percent = Math.Min(progress.Completed.Value / (double)progress.Total.Value, 1.0);

                Console.WriteLine($"Downloaded {completedMB:F1}/{totalMB:F1} MB ({percent:P1})");
            }

            await response.EnsureSuccessAsync();

            return Ok(new
            {
                message = $"Downloaded: {first?.Total / 1000000} MB",
                total = $"{first?.Total / 1000000} MB",
            });
        }
    }
}
