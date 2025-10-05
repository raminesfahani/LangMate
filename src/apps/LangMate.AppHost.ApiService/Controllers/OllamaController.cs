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

        /// <summary>
        /// Retrieves a list of all available Ollama models, optionally filtered by a search term.
        /// </summary>
        /// <param name="term">Optional search term to filter models by name or description.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>
        /// A filtered list of <see cref="OllamaModel"/> objects matching the search criteria.
        /// </returns>
        [HttpGet("models/available")]
        public async Task<IActionResult> GetModelListAsync(string term = "", CancellationToken cancellationToken = default)
        {
            var models = await _ollamaFactoryProvider.GetAvailableModelsListAsync();

            return Ok(models.Where(x => x.Name.Contains(term) || x.Description.Contains(term)));
        }

        /// <summary>
        /// Retrieves metadata for all locally installed Ollama models.
        /// </summary>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>
        /// A <see cref="ModelsResponse"/> containing details of installed models.
        /// </returns>
        [HttpGet("models/installed")]
        public async Task<IActionResult> GetLocalModels(CancellationToken cancellationToken)
        {
            var models = await _ollamaFactoryProvider.GetInstalledModelsAsync(cancellationToken);
            return Ok(models);
        }

        /// <summary>
        /// Initiates the download (pull) of a specified Ollama model and streams progress updates.
        /// </summary>
        /// <param name="model">The name or tag of the model to pull.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>
        /// An object containing the total downloaded size in MB and a status message.
        /// </returns>
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

            return Accepted(new
            {
                message = $"Downloaded: {first?.Total / 1000000} MB",
                total = $"{first?.Total / 1000000} MB",
            });
        }

        /// <summary>
        /// Delete a model from Ollama
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("models/pull/{model}")]
        public async Task<IActionResult> DeleteModelAsync([Required] string model, CancellationToken cancellationToken)
        {
            await _ollamaFactoryProvider.Client.Models.DeleteModelAsync(model, cancellationToken);            
            return NoContent();
        }
    }
}
