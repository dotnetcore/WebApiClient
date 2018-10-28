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
            var context = new ApiActionContext
            {
                HttpApiConfig = new HttpApiConfig(),
                RequestMessage = new HttpApiRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://www.webapi.com/")
                },
                ApiActionDescriptor = ApiActionDescriptorProvider.GetDescriptor(typeof(IMyApi).GetMethod("PostAsync"))
            };

            var parameter = context.ApiActionDescriptor.Parameters[0];
            parameter.Value = "http://www.baidu.com";

            var attr = new UriAttribute();
            await ((IApiParameterAttribute)attr).BeforeRequestAsync(context, parameter);
            Assert.True(context.RequestMessage.RequestUri == new Uri("http://www.baidu.com"));

            parameter.Value = "/login";
            await ((IApiParameterAttribute)attr).BeforeRequestAsync(context, parameter);
            Assert.True(context.RequestMessage.RequestUri == new Uri("http://www.baidu.com/login"));
        }
    }
}
