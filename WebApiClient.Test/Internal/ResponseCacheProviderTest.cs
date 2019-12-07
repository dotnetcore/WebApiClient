#if !NETSTANDARD1_3
using System;
using System.Threading.Tasks;
using WebApiClient.Defaults;
using Xunit;

namespace WebApiClient.Test.Internal
{
    public class ResponseCacheProviderTest
    {
        [Fact]
        public async Task GetSetAsyncTest()
        {
            var cache = await ResponseCacheProvider.Instance.GetAsync("key");
            Assert.False(cache.HasValue);

            await ResponseCacheProvider.Instance.SetAsync("key", new ResponseCacheEntry(), TimeSpan.FromMilliseconds(20));
            cache = await ResponseCacheProvider.Instance.GetAsync("key");
            Assert.True(cache.HasValue);

            await Task.Delay(20);
            cache = await ResponseCacheProvider.Instance.GetAsync("key");
            Assert.False(cache.HasValue);
        }
    }
}
#endif