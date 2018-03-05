using System;
using Xunit;
using WebApiClient;
using WebApiClient.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using WebApiClient.Defaults;

namespace WebApiClientTest.Defaults
{
    public class HttpClientTest
    {
        class MyHandler : System.Net.Http.DelegatingHandler
        {
            public MyHandler()
                : base(new System.Net.Http.HttpClientHandler())
            {
            }
        }

        class YourHandler : System.Net.Http.DelegatingHandler
        {
            public YourHandler(System.Net.Http.HttpMessageHandler handler)
                : base(handler)
            {
            }
        }

        [Fact]
        public void CtorTest()
        {
            var handler = new YourHandler(new MyHandler());
            var client = new HttpClient(handler);
            Assert.True(client.Handler != null);
        }
    }
}
