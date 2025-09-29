using Microsoft.AspNetCore.Builder;

namespace LangMate.Middleware.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseLangMateExceptionHandler(this IApplicationBuilder app) =>
            app.UseMiddleware<ExceptionHandlingMiddleware>();

        public static IApplicationBuilder UseLangMateResiliency(this IApplicationBuilder app) =>
            app.UseMiddleware<ResiliencyMiddleware>();

        public static IApplicationBuilder UseLangMateRequestLogging(this IApplicationBuilder app) =>
            app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
