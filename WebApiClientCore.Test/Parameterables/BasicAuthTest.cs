using System;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Parameters;
using Xunit;

namespace WebApiClientCore.Test.Parameterables
{
    public class BasicAuthTest
    {
        [Fact]
        public async Task OnRequestAsync()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, "value");

            var basicAuth = new BasicAuth("laojiu", "123456");
            await basicAuth.OnRequestAsync(new ApiParameterContext(context, 0));

            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes("laojiu:123456"));
            Assert.True(context.HttpContext.RequestMessage.Headers.Authorization.Parameter == auth);
        }
    }
}
