using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;

namespace WebApiClient.Test.Attributes.HttpActionAttributes
{
    public class UriAttributeTest
    {
        public interface IMyApi : IDisposable
        {
            ITask<HttpResponseMessage> PostAsync(string url);
        }

        [Fact]
        public async Task BeforeRequestAsyncTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync")));

            var parameter = context.ApiActionDescriptor.Parameters[0].Clone("http://www.baidu.com");

            var attr = new UriAttribute();
            await ((IApiParameterAttribute)attr).BeforeRequestAsync(context, parameter);
            Assert.True(context.RequestMessage.RequestUri == new Uri("http://www.baidu.com"));

            parameter = parameter.Clone("/login");
            await ((IApiParameterAttribute)attr).BeforeRequestAsync(context, parameter);
            Assert.True(context.RequestMessage.RequestUri == new Uri("http://www.baidu.com/login"));
        }
    }
}
