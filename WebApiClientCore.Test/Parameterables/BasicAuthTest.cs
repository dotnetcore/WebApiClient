using System;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Parameterables;
using Xunit;

namespace WebApiClientCore.Test.Parameterables
{
    public class BasicAuthTest
    {
        [Fact]
        public async Task BeforeRequestAsync()
        {
            var context = new TestActionContext(
                apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync")));
             
            var basicAuth = new BasicAuth("laojiu", "123456");
            await basicAuth.OnRequestAsync(new ApiParameterContext(context, context.ApiAction.Parameters[0],null));

            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes("laojiu:123456"));
            Assert.True(context.HttpContext.RequestMessage.Headers.Authorization.Parameter == auth);
        }
    }
}
