using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Contexts;
using WebApiClient.Interfaces;
using WebApiClient.Parameterables;
using Xunit;

namespace WebApiClientTest.Parameterables
{
    public class FormFieldTest
    {
        [Fact]
        public async Task Test()
        {
            var context = new ApiActionContext
            {
                RequestMessage = new HttpApiRequestMessage
                {
                    RequestUri = new Uri("http://www.mywebapi.com"),
                    Method = HttpMethod.Post
                },
                ApiActionDescriptor = ApiDescriptorCache.GetApiActionDescriptor(typeof(IMyApi).GetMethod("GetAsync"))
            };

            var parameter = context.ApiActionDescriptor.Parameters[0];
            IApiParameterable formField = new FormField("laojiu");
            await formField.BeforeRequestAsync(context, parameter);

            var body = await context.RequestMessage.ToStringAsync();
            Assert.Contains("name=laojiu", body);
        }
    }
}
