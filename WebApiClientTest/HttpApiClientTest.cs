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
        [HttpHost("http://localhost:5859")]
        public interface IMyApi : IHttpApiClient
        {
        }

        [Fact]
        public void CreateTest()
        {
            var client = HttpApiClient.Create<IMyApi>();
            Assert.True(client.ApiConfig != null);
            Assert.True(client.ApiInterceptor != null);            
        }
    }
}
