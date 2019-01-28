#if JIT
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using WebApiClient.Defaults;
using Xunit;

namespace WebApiClient.Test
{
    public class HttpApiFactoryTest
    {
        [Fact]
        public void CreateHttpApiTest()
        {
            var factory = new HttpApiFactory<IMyApi>()
                .SetLifetime(TimeSpan.FromMilliseconds(100d));

            var api1 = ((IHttpApiFactory)factory).CreateHttpApi();
            var api2 = ((IHttpApiFactory)factory).CreateHttpApi();
            Assert.True(IsHttpHandlerEquals(api1, api2));
            Assert.False(api1 == api2);

            Thread.Sleep(TimeSpan.FromMilliseconds(150));

            var api3 = ((IHttpApiFactory)factory).CreateHttpApi();
            Assert.False(IsHttpHandlerEquals(api1, api3));
        }

        [Fact]
        public void AddCreateTest()
        {
            HttpApiFactory.Add<IMyApi>()
               .SetLifetime(TimeSpan.FromMilliseconds(100d));

            var api1 = HttpApiFactory.Create<IMyApi>();
            var api2 = HttpApiFactory.Create<IMyApi>();
            Assert.True(IsHttpHandlerEquals(api1, api2));
            Assert.False(api1 == api2);

            Thread.Sleep(TimeSpan.FromMilliseconds(150));

            var api3 = HttpApiFactory.Create<IMyApi>();
            Assert.False(IsHttpHandlerEquals(api1, api3));
        }

        private bool IsHttpHandlerEquals(IHttpApi x, IHttpApi y)
        {
            return GetHttpHandler(x) == GetHttpHandler(y);
        }

        private HttpMessageHandler GetHttpHandler(IHttpApi httpApi)
        {
            var httpApiClient = httpApi as HttpApiClient;
            var interceptor = httpApiClient.ApiInterceptor as ApiInterceptor;
            return interceptor.HttpApiConfig.HttpHandler.SourceHanlder;
        }

        public interface IMyApi : IHttpApi
        {
        }
    }
}
#endif