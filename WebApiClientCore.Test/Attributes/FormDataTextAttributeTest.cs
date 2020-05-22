using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebApiClientCore.Attributes;
using Xunit;


namespace WebApiClientCore.Test.Attributes
{
    public class FormDataTextAttributeTest
    {
        private string get(string name, string value)
        {
            return $@"Content-Disposition: form-data; name=""{name}""

{HttpUtility.UrlEncode(value, Encoding.UTF8)}";
        }

        [Fact]
        public async Task OnRequestAsync_Parameter()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, "laojiu");
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;
            var parameterContext = new ApiParameterContext(context, 0);

            var attr = new FormDataTextAttribute();
            await attr.OnRequestAsync(parameterContext );
            var body = await context.HttpContext.RequestMessage.Content.ReadAsStringAsync();
            Assert.Contains(get("value", "laojiu"), body);
        }

        [Fact]
        public async Task OnRequestAsync()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, string.Empty);
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;

            var attr = new FormDataTextAttribute("value", "laojiu");
            await attr.OnRequestAsync(context);
            var body = await context.HttpContext.RequestMessage.Content.ReadAsStringAsync();
            Assert.Contains(get("value", "laojiu"), body);
        }
    }
}
