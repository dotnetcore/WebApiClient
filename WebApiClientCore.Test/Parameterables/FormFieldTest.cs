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

            context.HttpContext.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;
            
            IApiParameterable formField = new FormField("laojiu");
            await formField.OnRequestAsync(new ApiParameterContext(context,0));

            var body = await context.HttpContext.RequestMessage.Content.ReadAsStringAsync();
            Assert.Equal("name=laojiu", body);
        }
    }
}
