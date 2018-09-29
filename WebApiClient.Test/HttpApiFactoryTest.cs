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
            var factory = new HttpApiFactory <IMyApi>(null, null) { Lifetime = TimeSpan.FromMilliseconds(100) };
          
            var api1 = factory.CreateHttpApi();
            var api2 = factory.CreateHttpApi();
            Assert.True(IsHttpApiConfigEquals(api1, api2));
            Assert.False(api1 == api2);

            Thread.Sleep(TimeSpan.FromMilliseconds(150));

            var api3 = factory.CreateHttpApi();
            Assert.False(IsHttpApiConfigEquals(api1, api3));
        }

        private bool IsHttpApiConfigEquals(IHttpApiClient x, IHttpApiClient y)
        {
            var xInterceptor = x.ApiInterceptor as ApiInterceptor;
            var yInterceptor = y.ApiInterceptor as ApiInterceptor;

            return xInterceptor.HttpApiConfig == yInterceptor.HttpApiConfig &&
                xInterceptor.HttpApiConfig.HttpClient == yInterceptor.HttpApiConfig.HttpClient;
        }

        public interface IMyApi : IHttpApiClient
        {
        }
    }
}
