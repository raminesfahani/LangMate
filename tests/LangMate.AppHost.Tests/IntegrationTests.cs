using Aspire.Hosting;
using LangMate.Abstractions.Abstracts.Documents;
using LangMate.Abstractions.Contracts;
using LangMate.AppHost.ApiService.Models;
using LangMate.Core.Ollama;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using MongoDB.Bson;
using Moq;
using Ollama;
using OllamaSharp.Models;
using System.Net.Http.Json;
using System.Threading;
using Xunit.v3;

namespace LangMate.AppHost.Tests;

[Collection("SequentialTests")]
public class IntegrationTests : IAsyncLifetime
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(1);

    private DistributedApplication _appHost;
    private CancellationToken _cancellationToken;

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        (_appHost, _cancellationToken) = await SetupAppTestBuilder<Projects.LangMate_AppHost_AppHost>();
    }

    private static async Task<(DistributedApplication app, CancellationToken cancellationToken)> SetupAppTestBuilder<T>() where T : class
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<T>(cancellationToken);
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        return (app, cancellationToken);
    }

    [Fact]
    public async Task GetBlazorUIResourceRootReturnsOkStatusCode()
    {
        // Act
        var httpClient = _appHost.CreateHttpClient("langmate-ui");
        await _appHost.ResourceNotifications.WaitForResourceHealthyAsync("langmate-ui", _cancellationToken).WaitAsync(DefaultTimeout, _cancellationToken);
        var response = await httpClient.GetAsync("/", _cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetWebApiCoversationsReturnsOkStatusCode()
    {
        // Act
        var httpClient = _appHost.CreateHttpClient("langmate-api");
        await _appHost.ResourceNotifications.WaitForResourceHealthyAsync("langmate-api", _cancellationToken).WaitAsync(DefaultTimeout, _cancellationToken);
        var response = await httpClient.GetAsync("/Chat/conversations", _cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetWebApiOllamaInstalledModelsReturnsOkStatusCode()
    {
        // Act
        var httpClient = _appHost.CreateHttpClient("langmate-api");
        await _appHost.ResourceNotifications.WaitForResourceHealthyAsync("langmate-api", _cancellationToken).WaitAsync(DefaultTimeout, _cancellationToken);
        var response = await httpClient.GetAsync("/Ollama/models/installed", _cancellationToken).WithTimeout(DefaultTimeout);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ModelsResponse?>(cancellationToken: _cancellationToken);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetWebApiOllamaAvailableModelsReturnsOkStatusCode()
    {
        // Act
        var httpClient = _appHost.CreateHttpClient("langmate-api");
        await _appHost.ResourceNotifications.WaitForResourceHealthyAsync("langmate-api", _cancellationToken).WaitAsync(DefaultTimeout, _cancellationToken);

        var response = await httpClient.GetAsync("/Ollama/models/available", _cancellationToken).WithTimeout(DefaultTimeout);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<Ollama.Model>>(cancellationToken: _cancellationToken);
        Assert.NotNull(result);
    }

    async ValueTask System.IAsyncDisposable.DisposeAsync()
    {
        await _appHost.StopAsync(_cancellationToken);
        await _appHost.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
