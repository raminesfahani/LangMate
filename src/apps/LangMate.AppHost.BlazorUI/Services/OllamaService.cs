using LangMate.Abstractions.Contracts;
using Microsoft.AspNetCore.Mvc;
using Ollama;

namespace LangMate.AppHost.BlazorUI.Services;

public class OllamaService(IOllamaFactory ollamaFactoryProvider)
{
    private readonly IOllamaFactory _ollamaFactoryProvider = ollamaFactoryProvider;

    public async Task<IEnumerable<Abstractions.Abstracts.Documents.OllamaModel>> GetModelListAsync(string term = "")
    {
        var models = await _ollamaFactoryProvider.GetModelsListAsync();

        return models.Where(x => x.Name.Contains(term) || x.Description.Contains(term));
    }

    public async Task<ModelsResponse> GetLocalModels()
    {
        var models = await _ollamaFactoryProvider.GetAvailableModelsAsync();
        return models;
    }

    public async Task PullModelAsync(string model, Action<PullModelResponse?> onProgress, CancellationToken cancellationToken = default)
    {
        var response = _ollamaFactoryProvider.PullModelAsync(model, cancellationToken);
        PullModelResponse? first = null;

        await foreach (var progress in response.WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            // ✅ Let Blazor process UI events
            await Task.Yield();
            first ??= progress;

            if (first == null || progress.Completed == null || progress.Total == null)
                continue;

            double completedMB = progress.Completed.Value / 1_000_000.0;
            double totalMB = progress.Total.Value / 1_000_000.0;
            double percent = Math.Min(progress.Completed.Value / (double)progress.Total.Value, 1.0);

            Console.WriteLine($"Downloaded {completedMB:F1}/{totalMB:F1} MB ({percent:P1})");

            onProgress?.Invoke(progress);
        }

        await response.EnsureSuccessAsync();
    }

    public async Task DeletelModelAsync(string model, CancellationToken cancellationToken = default)
    {
        await _ollamaFactoryProvider.Client.Models.DeleteModelAsync(model, cancellationToken);
    }
}
