using System;
using WebApiClient;
using Xunit;

namespace WebApiClient.Test
{
    public class HttpApiConfigTest
    {
        [Fact]
        public static void Default_Properties_Test()
        {
            var config = new HttpApiConfig();

            Assert.True(config.FormatOptions != null);
            Assert.True(config.GlobalFilters != null);
            Assert.True(config.HttpHost == null);
            Assert.True(config.HttpClient != null);
            Assert.True(config.IsDisposed == false); 
            Assert.True(config.Tags != null);
        }

        [Fact]
        public static void DisposeTest()
        {
            var config = new HttpApiConfig();
            config.Dispose();
            Assert.True(config.IsDisposed);
        }
    }
}
