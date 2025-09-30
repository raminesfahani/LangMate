using LangMate.Abstractions.Contracts;
using LangMate.Abstractions.Options;
using LangMate.Middleware.Middlewares;
using LangMate.Middleware.Serilog;
using LangMate.Persistence;
using LangMate.Persistence.NoSQL.MongoDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LangMate.Middleware
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLangMateMiddleware(this IServiceCollection services, IConfiguration configuration, bool useApm = false)
        {
            ResiliencyMiddlewareOptions options = new();
            services.Configure<ResiliencyMiddlewareOptions>(configuration.GetSection(nameof(ResiliencyMiddlewareOptions)));
            configuration.GetSection(nameof(ResiliencyMiddlewareOptions)).Bind(options);

            services.AddLangMateMemoryCache();
            services.AddLangMateMongoDb(configuration);

            if (useApm == true)
                services.AddApm();

            return services;
        }

        public static IApplicationBuilder UseLangMateMiddleware(this IApplicationBuilder app, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            app.UseLogging(configuration, loggerFactory);
            app.UseLangMateRequestLogging();     // logs all incoming requests
            app.UseLangMateExceptionHandler();   // catch and serialize any errors
            app.UseLangMateResiliency();         // retry, timeout, circuit breaker

            return app;
        }
    }
}
