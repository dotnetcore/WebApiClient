using System;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using Xunit;

namespace WebApiClientCore.Test.Attributes.ActionAttributes
{

    public class BasicAuthAttributeTest
    {
        [Fact]
        public async Task OnRequestAsyncTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, string.Empty);

            var attr = new BasicAuthAttribute("laojiu", "123456");
            await attr.OnRequestAsync(context);

            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes("laojiu:123456"));
            Assert.True(context.HttpContext.RequestMessage.Headers.Authorization.Scheme == "Basic");
            Assert.True(context.HttpContext.RequestMessage.Headers.Authorization.Parameter == auth);
        }
    }
}
