using System;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Contexts;
using WebApiClient.Parameterables;
using Xunit;

namespace WebApiClient.Test.Parameterables
{
    public class BasicAuthTest
    {
        [Fact]
        public async Task Test()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: ApiActionDescriptor.Create(typeof(IMyApi).GetMethod("PostAsync")));

            var parameter = context.ApiActionDescriptor.Parameters[0];
            var basicAuth = new BasicAuth("laojiu", "123456");
            await basicAuth.BeforeRequestAsync(context, parameter);

            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes("laojiu:123456"));
            Assert.True(context.RequestMessage.Headers.Authorization.Parameter == auth);
        }
    }
}
