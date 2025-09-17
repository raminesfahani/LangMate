using LangMate.Abstractions.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace LangMate.Cache
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLangMateCache(this IServiceCollection services)
        {
            services.AddMemoryCache(); // ensures IMemoryCache is available
            services.AddScoped<ICacheProvider, MemoryCacheProvider>();
            return services;
        }
    }
}
