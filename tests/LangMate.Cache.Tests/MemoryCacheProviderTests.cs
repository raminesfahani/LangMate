using LangMate.Persistence.Cache.Memory;
using Microsoft.Extensions.Caching.Memory;
using System;
using Xunit;

namespace LangMate.Persistence.Tests
{
    public class MemoryCacheProviderTests
    {
        private static MemoryCacheProvider CreateProvider()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            return new MemoryCacheProvider(memoryCache);
        }

        [Fact]
        public void Set_StoresValueInCache()
        {
            var provider = CreateProvider();
            var key = "test-key";
            var value = "test-value";
            var ttl = TimeSpan.FromMinutes(5);

            provider.Set(key, value, ttl);

            Assert.True(provider.TryGet<string>(key, out var cachedValue));
            Assert.Equal(value, cachedValue);
        }

        [Fact]
        public void TryGet_ReturnsFalseIfKeyNotFound()
        {
            var provider = CreateProvider();
            var key = "missing-key";

            var found = provider.TryGet<string>(key, out var value);

            Assert.False(found);
            Assert.Null(value);
        }

        [Fact]
        public void Remove_DeletesValueFromCache()
        {
            var provider = CreateProvider();
            var key = "to-remove";
            var value = 123;
            var ttl = TimeSpan.FromMinutes(5);

            provider.Set(key, value, ttl);
            provider.Remove(key);

            Assert.False(provider.TryGet<int>(key, out var removedValue));
            Assert.Equal(default, removedValue);
        }
    }
}