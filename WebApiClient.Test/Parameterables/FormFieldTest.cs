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
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: ApiActionDescriptor.Create(typeof(IMyApi).GetMethod("PostAsync")));

            context.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.RequestMessage.Method = HttpMethod.Post;
           

            var parameter = context.ApiActionDescriptor.Parameters[0];
            IApiParameterable formField = new FormField("laojiu");
            await formField.BeforeRequestAsync(context, parameter);

            var body = await context.RequestMessage.Content.ReadAsStringAsync();
            Assert.Equal("name=laojiu", body);
        }
    }
}
