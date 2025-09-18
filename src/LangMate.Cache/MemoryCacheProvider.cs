using LangMate.Abstractions.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace LangMate.Cache
{
    public class MemoryCacheProvider(IMemoryCache cache) : ICacheProvider
    {
        private readonly IMemoryCache _cache = cache;

        public void Set<T>(string key, T value, TimeSpan ttl)
        {
            _cache.Set(key, value, ttl);
        }

        public bool TryGet<T>(string key, out T? value) => _cache.TryGetValue(key, out value);

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
