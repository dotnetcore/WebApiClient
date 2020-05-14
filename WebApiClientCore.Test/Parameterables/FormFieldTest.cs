using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Parameterables;
using Xunit;

namespace WebApiClientCore.Test.Parameterables
{
    public class FormFieldTest
    {
        [Fact]
        public async Task BeforeRequestAsync()
        {
            var context = new TestActionContext(
                apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync")));

            context.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.RequestMessage.Method = HttpMethod.Post;

            var parameter = context.ApiAction.Parameters[0];
            IApiParameterable formField = new FormField("laojiu");
            await formField.BeforeRequestAsync(new ApiParameterContext(context, parameter));

            var body = await context.RequestMessage.Content.ReadAsStringAsync();
            Assert.Equal("name=laojiu", body);
        }
    }
}
