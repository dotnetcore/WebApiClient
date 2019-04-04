#if JIT

using System;
using System.Net.Http;
using System.Threading;
using WebApiClient;
using WebApiClient.Defaults;
using Xunit;

namespace WebApiClient.Test
{
    public class HttpApiTest
    {
        public interface IMyApi : IHttpApi
        {
        }

        public interface IMyApi2 : IHttpApi
        {
            int Value { get; set; }
        }

        public interface IMyApi3 : IHttpApi
        {
            int GetValue();
        }

        public interface IMyApi4 : IHttpApi
        {
            ITask<T> GetValue<T>();
        }

        public class MyApi : IHttpApi
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void CreateTest()
        {
            var api = HttpApi.Create<IMyApi>();
            Assert.True(api != null);
            Assert.Throws<ArgumentException>(() => HttpApi.Create<MyApi>());
            Assert.Throws<NotSupportedException>(() => HttpApi.Create<IMyApi2>());
            Assert.Throws<NotSupportedException>(() => HttpApi.Create<IMyApi3>());
            Assert.Throws<NotSupportedException>(() => HttpApi.Create<IMyApi4>());
        }

        [Fact]
        public void Register_Get_Test()
        {
            HttpApi.Register<IMyApi>()
               .SetLifetime(TimeSpan.FromMilliseconds(100d));

            var api1 = HttpApi.Resolve<IMyApi>();
            var api2 = HttpApi.Resolve<IMyApi>();
            Assert.True(IsHttpHandlerEquals(api1, api2));
            Assert.False(api1 == api2);

            Thread.Sleep(TimeSpan.FromMilliseconds(150));

            var api3 = HttpApi.Resolve<IMyApi>();
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
    }
}
#endif