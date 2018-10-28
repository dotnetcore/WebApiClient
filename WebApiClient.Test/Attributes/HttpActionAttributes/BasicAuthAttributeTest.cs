using System;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;


namespace WebApiClient.Test.Attributes.HttpActionAttributes
{
    public class BasicAuthAttributeTest
    {
        [Fact]
        public async Task BeforeRequestAsyncTest()
        {
            var context = new ApiActionContext
            {
                RequestMessage = new HttpApiRequestMessage(),
                ApiActionDescriptor = ApiActionDescriptorProvider.GetDescriptor(typeof(IMyApi).GetMethod("PostAsync"))
            };

            var attr = new BasicAuthAttribute("laojiu", "123456");
            await attr.BeforeRequestAsync(context);

            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes("laojiu:123456"));
            Assert.True(context.RequestMessage.Headers.Authorization.Parameter == auth);
        }
    }
}
