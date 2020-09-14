using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace WebApiClientCore.Test.Microsoft.Extensions.DependencyInjection
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

            var name = HttpApi.GetName(typeof(IDiApi));
            var options = services.GetService<IOptionsMonitor<HttpApiOptions>>().Get(name);
            Assert.True(options.HttpHost == host);
        } 

        [Fact]
        public static void ConfigureHttpApiNoGenericTest()
        {
            var di = new ServiceCollection();
            var host = new Uri("http://www.x.com");
            di.AddHttpApi<IDiApi>();
            di.ConfigureHttpApi(typeof(IDiApi), o => o.HttpHost = host);
            var services = di.BuildServiceProvider();

            var name = HttpApi.GetName(typeof(IDiApi));
            var options = services.GetService<IOptionsMonitor<HttpApiOptions>>().Get(name);
            Assert.True(options.HttpHost == host);

            Assert.Null(services.GetService<IOptions<HttpApiOptions>>().Value.HttpHost);
        }


        public interface IDiApi
        {
        }
    }
}
