using System;
using WebApiClient;
using Xunit;

namespace WebApiClientTest
{
    public class HttpApiClientTest
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

        public class MyApi : IDisposable
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void CreateTest()
        {
            var client = HttpApiClient.Create<IMyApi>();
            Assert.True(client.ApiConfig != null);
            Assert.True(client.ApiInterceptor != null);
            Assert.Throws<ArgumentException>(() => HttpApiClient.Create<MyApi>());
            Assert.Throws<NotSupportedException>(() => HttpApiClient.Create<IMyApi2>());
            Assert.Throws<NotSupportedException>(() => HttpApiClient.Create<IMyApi3>());
            Assert.Throws<NotSupportedException>(() => HttpApiClient.Create<IMyApi4>());
        }
    }
}
