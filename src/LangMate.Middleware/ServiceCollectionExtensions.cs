using LangMate.Abstractions.Contracts;
using LangMate.Middleware.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace LangMate.Middleware
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLangMateMiddleware(this IServiceCollection services, IConfiguration configuration, bool useApm = false)
        {
            services.AddScoped<IMiddlewareProvider, MiddlewareProvider>();

            if (useApm == true)
                services.AddApm();

            return services;
        }

        public static IApplicationBuilder UseLangMateMiddleware(this IApplicationBuilder app, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            app.UseLogging(configuration, loggerFactory);

            return app;
        }
    }
}
