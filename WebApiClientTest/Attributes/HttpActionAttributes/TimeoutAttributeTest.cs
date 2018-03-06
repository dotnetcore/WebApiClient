using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;


namespace WebApiClientTest.Attributes.HttpActionAttributes
{
    public class TimeoutAttributeTest
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

            var attr = new TimeoutAttribute(5000);
            await attr.BeforeRequestAsync(context);
            Assert.True(context.RequestMessage.Timeout == TimeSpan.FromSeconds(5));
        }
    }
}
