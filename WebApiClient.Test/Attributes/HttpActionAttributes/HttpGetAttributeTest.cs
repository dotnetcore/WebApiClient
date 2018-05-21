using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;


namespace WebApiClient.Test.Attributes.HttpActionAttributes
{
    public class HttpGetAttributeTest
    {
        [Fact]
        public async Task BeforeRequestAsyncTest()
        {
            var context = new ApiActionContext
            {
                RequestMessage = new HttpApiRequestMessage
                {
                    RequestUri = new Uri("http://www.webapi.com/")
                },
                ApiActionDescriptor = ApiDescriptorCache.GetApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"))
            };

            var attr = new HttpGetAttribute();
            await attr.BeforeRequestAsync(context);
            Assert.True(context.RequestMessage.Method == HttpMethod.Get);

            var attr2 = new HttpGetAttribute("/login");
            await attr2.BeforeRequestAsync(context);
            Assert.True(context.RequestMessage.Method == HttpMethod.Get);
            Assert.True(context.RequestMessage.RequestUri == new Uri("http://www.webapi.com/login"));

            var attr3 = new HttpGetAttribute("http://www.baidu.com");
            await attr3.BeforeRequestAsync(context);
            Assert.True(context.RequestMessage.Method == HttpMethod.Get);
            Assert.True(context.RequestMessage.RequestUri == new Uri("http://www.baidu.com"));
        }
    }
}
