using LangMate.Abstractions.Abstracts.Persistence;
using LangMate.Abstractions.Abstracts.Settings;
using LangMate.Abstractions.Contracts;
using LangMate.Persistence.Cache.Memory;
using LangMate.Persistence.NoSQL.MongoDB.Repository;
using LangMate.Persistence.NoSQL.MongoDB.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LangMate.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLangMateMemoryCache(this IServiceCollection services)
        {
            services.AddMemoryCache(); // ensures IMemoryCache is available
            services.AddScoped<ICacheProvider, MemoryCacheProvider>();
            return services;
        }

        public static IServiceCollection AddLangMateMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbSettings>(settings => configuration.GetSection(nameof(MongoDbSettings)).Bind(settings));

            services.AddSingleton<IMongoDbSettings>(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);

            services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

            return services;
        }
    }
}
