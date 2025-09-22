using LangMate.Abstractions.Abstracts.Settings;
using LangMate.Abstractions.Contracts;
using LangMate.Abstractions.Options;
using LangMate.Core.Providers;
using LangMate.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ollama;

namespace LangMate.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLangMateCore(this IServiceCollection services, IConfiguration configuration, bool useApm = false)
        {
            OllamaOptions options = new();
            services.Configure<OllamaOptions>(configuration.GetSection(nameof(OllamaOptions)));
            configuration.GetSection(nameof(OllamaOptions)).Bind(options);

            services.AddLangMateMiddleware(configuration, useApm);

            services.AddSingleton<IOllamaApiClient>(factory =>
            {
                return new OllamaApiClient(baseUri: new Uri(options.Endpoint));
            });

            services.AddScoped<IOllamaScraper, OllamaScraper>();
            services.AddScoped<IOllamaFactoryProvider, OllamaFactoryProvider>();

            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(typeof(AppSettingsBase).Assembly,
                            typeof(Persistence.ServiceCollectionExtensions).Assembly,
                            typeof(LangMate.Core.ServiceCollectionExtensions).Assembly,
                            typeof(LangMate.Middleware.ServiceCollectionExtensions).Assembly
                            );
            });

            return services;
        }

        public static IApplicationBuilder UseLangMateCore(this IApplicationBuilder app, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            app.UseLangMateMiddleware(configuration, loggerFactory);

            return app;
        }
    }
}
