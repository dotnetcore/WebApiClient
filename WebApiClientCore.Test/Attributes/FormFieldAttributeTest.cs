using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using Xunit;


namespace WebApiClientCore.Test.Attributes
{
    public class FormFieldAttributeTest
    {
        [Fact]
        public async Task OnRequestAsync_Parameter()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, "laojiu");
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;
            var parameterContext = new ApiParameterContext(context, 0);

            var attr = new FormFieldAttribute();
            await attr.OnRequestAsync(parameterContext);
            var body = await context.HttpContext.RequestMessage.Content.ReadAsStringAsync();
            Assert.Equal("value=laojiu", body);
        }

        [Fact]
        public async Task OnRequestAsync()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, string.Empty);
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;

            var attr = new FormFieldAttribute("value", "laojiu");
            await attr.OnRequestAsync(context);
            var body = await context.HttpContext.RequestMessage.Content.ReadAsStringAsync();
            Assert.Equal("value=laojiu", body);
        }
    }
}
