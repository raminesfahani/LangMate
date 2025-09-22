using LangMate.Abstractions.Contracts;
using LangMate.Core.Providers;
using LangMate.Persistence.NoSQL.MongoDB.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ollama;
using System;
using System.ComponentModel.DataAnnotations;

namespace LangMate.Serve.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OllamaController(ILogger<OllamaController> logger, IOllamaFactoryProvider ollamaFactoryProvider) : ControllerBase
    {
        private readonly ILogger<OllamaController> _logger = logger;
        private readonly IOllamaFactoryProvider _ollamaFactoryProvider = ollamaFactoryProvider;

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
        public async Task<IActionResult> PullModelAsync([Required]string model, CancellationToken cancellationToken)
        {
            var response = _ollamaFactoryProvider.PullModelAsync(model, cancellationToken);

            var result = "";
            PullModelResponse? lastItem = null;
            await foreach (var progress in response)
            {
                lastItem = progress;

                if (lastItem == null)
                    continue;

                result = $"Downloaded: {progress.Completed / 1000000.0}/{progress.Total / 1000000.0} MB";
                Console.WriteLine(result);
            }

            return Ok(new
            {
                message = result,
                response = lastItem,
            });
        }
    }
}
