using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using WebApiClient.Interfaces;
using Xunit;

namespace WebApiClientTest.Attributes.HttpActionAttributes
{
    public class HttpContentAttributeTest
    {
        public interface IMyApi : IDisposable
        {
            ITask<HttpResponseMessage> PostAsync(HttpContent content);
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
                ApiActionDescriptor = ApiDescriptorCache.GetApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"))
            };

            var parameter = context.ApiActionDescriptor.Parameters[0];
            parameter.Value = new StringContent("laojiu");
            var attr = new HttpContentAttribute();
            await ((IApiParameterAttribute)attr).BeforeRequestAsync(context, parameter);

            var body = await context.RequestMessage.Content.ReadAsStringAsync();
            Assert.True(body == "laojiu");
        }
    }
}