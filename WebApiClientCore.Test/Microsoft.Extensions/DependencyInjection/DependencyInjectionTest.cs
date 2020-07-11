using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using WebApiClientCore.Serialization;
using Xunit;
using System.Linq;

namespace WebApiClientCore.Test.Microsoft.Extensions.DependencyInjection
{
    public class DependencyInjectionTest
    {
        public interface IDiApi
        {
        }

        public interface IDiApi2
        {
        }

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

            var options = services.GetService<IOptionsMonitor<HttpApiOptions>>().Get(typeof(IDiApi).FullName);
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

            var options = services.GetService<IOptionsMonitor<HttpApiOptions>>().Get(typeof(IDiApi).FullName);
            Assert.True(options.HttpHost == host);

            Assert.Null(services.GetService<IOptions<HttpApiOptions>>().Value.HttpHost);
        }

        //测试替换为NewtonsoftJsonSerializer
        [Fact]
        public static void ConfigureHttpApiJsonSerializerReplaceToNewtonsoftJsonSerializerTest()
        {
            var di = new ServiceCollection();
            di.AddHttpApi<IDiApi>();
            di.ConfigureHttpApiUseNewtonsoftJson();
            var services = di.BuildServiceProvider();

            IJsonSerializer jsonSerializer = services.GetService<IJsonSerializer>();
            Assert.IsType<NewtonsoftJsonSerializer>(jsonSerializer);
        }

        //测试IJsonSerializer只有一个，且为NewtonsoftJsonSerializer
        [Fact]
        public static void ConfigureHttpApiJsonSerializerOnlyNewtonsoftJsonSerializer()
        {
            var di = new ServiceCollection();
            di.AddHttpApi<IDiApi>();
            di.AddHttpApi<IDiApi2>();
            di.ConfigureHttpApiUseNewtonsoftJson();

            int count = di.Count(t => t.ServiceType == typeof(IJsonSerializer));
            Assert.Equal(1, count);

            var services = di.BuildServiceProvider();
            IJsonSerializer jsonSerializer = services.GetService<IJsonSerializer>();
            Assert.IsType<NewtonsoftJsonSerializer>(jsonSerializer);
        }
    }
}