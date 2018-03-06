using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;


namespace WebApiClientTest.Attributes.HttpActionAttributes
{
    public class HttpHostAttributeTest
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

            Assert.Throws<ArgumentNullException>(() => new HttpHostAttribute(null));
            Assert.Throws<UriFormatException>(() => new HttpHostAttribute("/"));

            var attr = new HttpHostAttribute("http://www.webapiclient.com");
            await attr.BeforeRequestAsync(context);
            Assert.True(context.RequestMessage.RequestUri == new Uri("http://www.webapiclient.com"));
        }
    }
}
