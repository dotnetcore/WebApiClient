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
            var factory = new HttpApiFactory { Lifetime = TimeSpan.FromMilliseconds(100) };
            factory.AddHttpApi<IMyApi>(null, null);

            var api1 = factory.CreateHttpApi<IMyApi>();
            var api2 = factory.CreateHttpApi<IMyApi>();
            Assert.True(IsHandlerEquals(api1, api2));
            Assert.False(api1 == api2);

            Thread.Sleep(TimeSpan.FromMilliseconds(150));

            var api3 = factory.CreateHttpApi<IMyApi>();
            Assert.False(IsHandlerEquals(api1, api3));
        }

        private bool IsHandlerEquals(IHttpApiClient x, IHttpApiClient y)
        {
            var xInterceptor = x.ApiInterceptor as ApiInterceptor;
            var yInterceptor = y.ApiInterceptor as ApiInterceptor;

            var xHandler = xInterceptor.HttpApiConfig.HttpHandler.SourceHanlder as DelegatingHandler;
            var yHandler = yInterceptor.HttpApiConfig.HttpHandler.SourceHanlder as DelegatingHandler;

            return xHandler.InnerHandler == yHandler.InnerHandler;
        }

        public interface IMyApi : IHttpApiClient
        {
        }
    }
}
