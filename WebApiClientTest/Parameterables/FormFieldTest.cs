using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Contexts;
using WebApiClient.Parameterables;
using Xunit;

namespace WebApiClient.Test.Parameterables
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
                ApiActionDescriptor = ApiDescriptorCache.GetApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"))
            };

            var parameter = context.ApiActionDescriptor.Parameters[0];
            IApiParameterable formField = new FormField("laojiu");
            await formField.BeforeRequestAsync(context, parameter);

            var body = await context.RequestMessage.Content.ReadAsStringAsync();
            Assert.Equal("name=laojiu", body);
        }
    }
}
