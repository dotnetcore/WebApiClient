using System;
using Xunit;
using WebApiClient;
using WebApiClient.Interfaces;

namespace WebApiClientTest
{
    public class HttpApiConfigTest
    {
        [Fact]
        public static void Static_Properties_Test()
        {
            Assert.True(HttpApiConfig.DefaultJsonFormatter != null && HttpApiConfig.DefaultJsonFormatter is IJsonFormatter);
            Assert.True(HttpApiConfig.DefaultKeyValueFormatter != null && HttpApiConfig.DefaultKeyValueFormatter is IKeyValueFormatter);
            Assert.True(HttpApiConfig.DefaultXmlFormatter != null && HttpApiConfig.DefaultXmlFormatter is IXmlFormatter);
        }

        [Fact]
        public static void Default_Properties_Test()
        {
            var config = new HttpApiConfig();

            Assert.True(config.FormatOptions != null);
            Assert.True(config.GlobalFilters != null);
            Assert.True(config.HttpHost == null);
            Assert.True(config.HttpClient != null);
            Assert.True(config.IsDisposed == false);
            Assert.True(config.JsonFormatter == HttpApiConfig.DefaultJsonFormatter);
            Assert.True(config.KeyValueFormatter == HttpApiConfig.DefaultKeyValueFormatter);
            Assert.True(config.XmlFormatter == HttpApiConfig.DefaultXmlFormatter);
            Assert.True(config.Tags != null);
        }

        [Fact]
        public static void DisposeTest()
        {
            var config = new HttpApiConfig();
            config.Dispose();

            Assert.True(config.IsDisposed);
            Assert.Throws<ObjectDisposedException>(() => config.HttpClient);
        }
    }
}
