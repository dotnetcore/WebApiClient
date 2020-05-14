#if !NETSTANDARD1_3
using System;
using System.Threading.Tasks;
using WebApiClientCore.Defaults;
using Xunit;

namespace WebApiClientCore.Test.BuildinUtils
{
    public class ResponseCacheProviderTest
    {
        [Fact]
        public async Task GetSetAsyncTest()
        {
            var provider = new ResponseCacheProvider();
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