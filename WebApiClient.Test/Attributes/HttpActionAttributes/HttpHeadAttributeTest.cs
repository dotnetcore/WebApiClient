using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;


namespace WebApiClient.Test.Attributes.HttpActionAttributes
{
    public class HttpHeadAttributeTest
    {
        [Fact]
        public async Task BeforeRequestAsyncTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: ApiActionDescriptor.Create(typeof(IMyApi).GetMethod("PostAsync")));

            context.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.RequestMessage.Method = HttpMethod.Post; 

            var attr = new HttpHeadAttribute();
            await attr.BeforeRequestAsync(context);
            Assert.True(context.RequestMessage.Method == HttpMethod.Head);

            var attr2 = new HttpHeadAttribute("/login");
            await attr2.BeforeRequestAsync(context);
            Assert.True(context.RequestMessage.Method == HttpMethod.Head);
            Assert.True(context.RequestMessage.RequestUri == new Uri("http://www.webapi.com/login"));

            var attr3 = new HttpHeadAttribute("http://www.baidu.com");
            await attr3.BeforeRequestAsync(context);
            Assert.True(context.RequestMessage.Method == HttpMethod.Head);
            Assert.True(context.RequestMessage.RequestUri == new Uri("http://www.baidu.com"));
        }
    }
}
