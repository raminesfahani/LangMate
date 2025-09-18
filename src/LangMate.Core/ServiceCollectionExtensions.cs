using AutoMapper;
using LangMate.Abstractions;
using LangMate.Abstractions.Contracts;
using LangMate.Abstractions.Dto;
using LangMate.Abstractions.Options;
using LangMate.Middleware;
using LangMate.Providers.LocalAI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LangMate.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLangMateCore(this IServiceCollection services, IConfiguration configuration, bool useApm = false)
        {
            AIOptions options = new();
            services.Configure<AIOptions>(configuration.GetSection(nameof(AIOptions)));
            configuration.GetSection(nameof(AIOptions)).Bind(options);

            services.AddLangMateMiddleware(configuration, useApm)
                    .AddLocalAIServices(configuration);

            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(typeof(LangMate.Abstractions.AppSettingsBase).Assembly,
                            typeof(LangMate.Cache.ServiceCollectionExtensions).Assembly,
                            typeof(LangMate.Core.ServiceCollectionExtensions).Assembly,
                            typeof(LangMate.Middleware.ServiceCollectionExtensions).Assembly,
                            typeof(LangMate.Providers.LocalAI.ServiceCollectionExtensions).Assembly
                            );
            });


            /// Register the AI client as a singleton
            /// from the AI options provided.
            services.AddSingleton(factory =>
            {
                var mapper = factory.GetRequiredService<IMapper>();
                return AIClientFactory.CreateClient(options, mapper);
            });

            services.AddScoped<IAICoreService, AICoreService>();

            return services;
        }

        public static IApplicationBuilder UseLangMateCore(this IApplicationBuilder app, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            app.UseLangMateMiddleware(configuration, loggerFactory);

            return app;
        }
    }
}
