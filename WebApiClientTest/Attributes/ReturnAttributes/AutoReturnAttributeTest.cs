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
    public class AutoReturnAttributeTest
    {
        public interface IMyApi : IDisposable
        {
            ITask<HttpResponseMessage> HttpResponseMessageAsync();

            ITask<string> StringAsync();
        }

        [Fact]
        public async Task HttpResponseMessageAsyncTest()
        {
            var context = new ApiActionContext
            {
                HttpApiConfig = new HttpApiConfig(),
                RequestMessage = new HttpApiRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://www.webapi.com/")
                },
                ResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK),
                ApiActionDescriptor = ApiDescriptorCache
                .GetApiActionDescriptor(typeof(IMyApi)
                .GetMethod("HttpResponseMessageAsync"))
            };
            context.ResponseMessage.Content = new StringContent("laojiu");

            var attr = new AutoReturnAttribute();
            var result = await ((IApiReturnAttribute)attr).GetTaskResult(context);
            Assert.True(result is HttpResponseMessage);
        }

        [Fact]
        public async Task StringAsyncTest()
        {
            var context = new ApiActionContext
            {
                HttpApiConfig = new HttpApiConfig(),
                RequestMessage = new HttpApiRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://www.webapi.com/")
                },
                ResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK),
                ApiActionDescriptor = ApiDescriptorCache
                .GetApiActionDescriptor(typeof(IMyApi)
                .GetMethod("StringAsync"))
            };
            context.ResponseMessage.Content = new StringContent("laojiu");

            var attr = new AutoReturnAttribute();
            var result = await ((IApiReturnAttribute)attr).GetTaskResult(context);
            Assert.True(result?.ToString() == "laojiu");
        }        
    }
}