#if !NETSTANDARD1_3
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using WebApiClientCore.ResponseCaches;
using Xunit;

namespace WebApiClientCore.Test.ResponseCaches
{
    public class ResponseCacheProviderTest
    {
        [Fact]
        public async Task GetSetAsyncTest()
        {
            var options = Options.Create(new MemoryCacheOptions());
            var memCache = new MemoryCache(options);
            var provider = new ResponseCacheProvider(memCache);
            var cache = await provider.GetAsync("key");
            Assert.False(cache.HasValue);

            await provider.SetAsync("key", new ResponseCacheEntry(), TimeSpan.FromSeconds(1d));
            cache = await provider.GetAsync("key");
            Assert.True(cache.HasValue);

            await Task.Delay(TimeSpan.FromSeconds(1.1d));
            cache = await provider.GetAsync("key");
            Assert.False(cache.HasValue);
        }
    }
}
#endif