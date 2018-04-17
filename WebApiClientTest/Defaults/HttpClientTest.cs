using WebApiClient.Defaults;
using Xunit;

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
            Assert.True(client.Handler.InnerHanlder.GetType() == typeof(System.Net.Http.HttpClientHandler));
        }
    }
}
