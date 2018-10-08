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

            var api1 = factory.CreateHttpApi();
            var api2 = factory.CreateHttpApi();
            Assert.True(IsHttpApiConfigEquals(api1, api2));
            Assert.False(api1 == api2);

            Thread.Sleep(TimeSpan.FromMilliseconds(150));

            var api3 = factory.CreateHttpApi();
            Assert.False(IsHttpApiConfigEquals(api1, api3));

            api3.Dispose();
            Assert.True(GetHttpApiConfig(api3).IsDisposed == false);
        }

        [Fact]
        public void AddCreateTest()
        {
            HttpApiFactory.Add<IMyApi>()
               .SetLifetime(TimeSpan.FromMilliseconds(100d));

            var api1 = HttpApiFactory.Create<IMyApi>();
            var api2 = HttpApiFactory.Create<IMyApi>();
            Assert.True(IsHttpApiConfigEquals(api1, api2));
            Assert.False(api1 == api2);

            Thread.Sleep(TimeSpan.FromMilliseconds(150));

            var api3 = HttpApiFactory.Create<IMyApi>();
            Assert.False(IsHttpApiConfigEquals(api1, api3));

            api3.Dispose();
            Assert.True(GetHttpApiConfig(api3).IsDisposed == false);
        }

        private bool IsHttpApiConfigEquals(IHttpApi x, IHttpApi y)
        {
            return GetHttpApiConfig(x) == GetHttpApiConfig(y);
        }

        private HttpApiConfig GetHttpApiConfig(IHttpApi httpApi)
        {
            var httpApiClient = httpApi as HttpApiClient;
            var interceptor = httpApiClient.ApiInterceptor as ApiInterceptor;
            return interceptor.HttpApiConfig;
        }

        public interface IMyApi : IHttpApi
        {
        }
    }
}
#endif