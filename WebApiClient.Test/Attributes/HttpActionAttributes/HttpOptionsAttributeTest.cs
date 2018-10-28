using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;


namespace WebApiClient.Test.Attributes.HttpActionAttributes
{
    public class HttpOptionsAttributeTest
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
                ApiActionDescriptor = ApiActionDescriptorProvider.GetDescriptor(typeof(IMyApi).GetMethod("PostAsync"))
            };

            var attr = new HttpOptionsAttribute();
            await attr.BeforeRequestAsync(context);
            Assert.True(context.RequestMessage.Method == HttpMethod.Options);

            var attr2 = new HttpOptionsAttribute("/login");
            await attr2.BeforeRequestAsync(context);
            Assert.True(context.RequestMessage.Method == HttpMethod.Options);
            Assert.True(context.RequestMessage.RequestUri == new Uri("http://www.webapi.com/login"));

            var attr3 = new HttpOptionsAttribute("http://www.baidu.com");
            await attr3.BeforeRequestAsync(context);
            Assert.True(context.RequestMessage.Method == HttpMethod.Options);
            Assert.True(context.RequestMessage.RequestUri == new Uri("http://www.baidu.com"));
        }
    }
}
