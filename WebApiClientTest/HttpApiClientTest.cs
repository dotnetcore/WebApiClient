using System;
using Xunit;
using WebApiClient;
using WebApiClient.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;

namespace WebApiClientTest
{
    public class HttpApiClientTest
    {
        public interface IMyApi : IHttpApiClient
        {
        }

        public interface IMyApi2 : IHttpApiClient
        {
            int Value { get; set; }
        }

        public interface IMyApi3 : IHttpApiClient
        {
            int GetValue();
        }

        public interface IMyApi4 : IHttpApiClient
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
