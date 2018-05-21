using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;


namespace WebApiClient.Test.Attributes.HttpActionAttributes
{
    public class ProxyAttributeTest
    {
        [Fact]
        public async Task BeforeRequestAsyncTest()
        {
            var context = new ApiActionContext
            {
                HttpApiConfig = new HttpApiConfig(),
                RequestMessage = new HttpApiRequestMessage(),
                ApiActionDescriptor = ApiDescriptorCache.GetApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"))
            };

            var attr = new ProxyAttribute("localhost", 5555);
            await attr.BeforeRequestAsync(context);
            var handler = context.HttpApiConfig.HttpClient.Handler;
            Assert.True(handler.UseProxy == true);
            Assert.True(handler.Proxy != null);
            Assert.True(handler.Proxy.Credentials == null);
            Assert.True(handler.Proxy.GetProxy(new Uri("http://www.baidu.com")).Authority == "localhost:5555");

            var attr2 = new ProxyAttribute("localhost", 5555, "laojiu", "123456");
            await attr2.BeforeRequestAsync(context);
            handler = context.HttpApiConfig.HttpClient.Handler;
            Assert.True(handler.UseProxy == true);
            Assert.True(handler.Proxy != null);
            Assert.True(handler.Proxy.GetProxy(new Uri("http://www.baidu.com")).Authority == "localhost:5555");
            Assert.True(handler.Proxy.Credentials != null);
        }
    }
}
