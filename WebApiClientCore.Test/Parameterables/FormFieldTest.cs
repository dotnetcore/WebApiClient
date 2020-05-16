using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Parameters;
using Xunit;

namespace WebApiClientCore.Test.Parameterables
{
    public class FormFieldTest
    {
        [Fact]
        public async Task OnRequestAsync()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, "value");

            context.HttpContext.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;
            
            IApiParameterable formField = new FormField("laojiu");
            await formField.OnRequestAsync(new ApiParameterContext(context,0));

            var body = await context.HttpContext.RequestMessage.Content.ReadAsStringAsync();
            Assert.Equal("value=laojiu", body);
        }
    }
}
