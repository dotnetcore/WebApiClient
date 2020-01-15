
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using WebApiClient.Defaults;
using Xunit;

namespace WebApiClient.Test
{
    public class DependencyInjectionTest
    {

        [Fact]
        public static void AddHttpApiTest()
        {
            var di = new ServiceCollection();
            di.AddHttpApi<IDiApi>();
            var services = di.BuildServiceProvider();

            var api = services.GetService<IDiApi>();
            Assert.True(api != null);
        }

        [Fact]
        public static void AddHttpApiNoGenericTest()
        {
            var di = new ServiceCollection();
            di.AddHttpApi(typeof(IDiApi));
            var services = di.BuildServiceProvider();

            var api = services.GetService<IDiApi>();
            Assert.True(api != null);
        }

        [Fact]
        public static void ConfigureHttpApiTest()
        {
            var di = new ServiceCollection();
            var host = new Uri("http://www.x.com");
            di.AddHttpApi<IDiApi>();
            di.ConfigureHttpApi<IDiApi>(o => o.HttpHost = host);
            var services = di.BuildServiceProvider();

            var api = services.GetService<IDiApi>();
            var config = ((ApiInterceptor)((HttpApi)api).ApiInterceptor).HttpApiConfig;
            Assert.True(config.HttpHost == host);
        }


        [Fact]
        public static void ConfigureHttpApiNoGenericTest()
        {
            var di = new ServiceCollection();
            var host = new Uri("http://www.x.com");
            di.AddHttpApi<IDiApi>();
            di.ConfigureHttpApi(typeof(IDiApi), o => o.HttpHost = host);
            var services = di.BuildServiceProvider();

            var api = services.GetService<IDiApi>();
            var config = ((ApiInterceptor)((HttpApi)api).ApiInterceptor).HttpApiConfig;
            Assert.True(config.HttpHost == host);
            Assert.Null(services.GetService<IOptions<HttpApiOptions>>().Value.HttpHost);
            Assert.NotNull(services.GetService<IOptions<HttpApiOptions<IDiApi>>>().Value.HttpHost);
        }

        public interface IDiApi : IHttpApi
        {

        }
    }
}