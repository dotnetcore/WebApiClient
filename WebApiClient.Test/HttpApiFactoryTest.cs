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

        private bool IsHttpHandlerEquals(IHttpApi x, IHttpApi y)
        {
            return GetHttpHandler(x) == GetHttpHandler(y);
        }

        private HttpMessageHandler GetHttpHandler(IHttpApi ihttpApi)
        {
            var httpApi = ihttpApi as HttpApi;
            var interceptor = httpApi.ApiInterceptor as ApiInterceptor;
            return interceptor.HttpApiConfig.HttpHandler.SourceHanlder;
        }
        public interface IMyApi : IHttpApi
        {
        }
    }
}
#endif