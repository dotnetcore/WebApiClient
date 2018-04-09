using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Contexts;
using WebApiClient.Parameterables;
using Xunit;

namespace WebApiClientTest.Parameterables
{
    public class TimeoutTest
    {
        [Fact]
        public async Task Test()
        {
            var context = new ApiActionContext
            {
                RequestMessage = new HttpApiRequestMessage
                {
                    RequestUri = new Uri("http://www.mywebapi.com"),
                    Method = HttpMethod.Post
                },
                ApiActionDescriptor = ApiDescriptorCache.GetApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"))
            };

            var parameter = context.ApiActionDescriptor.Parameters[0];
            IApiParameterable timeout = new TimeOut(5000);
            await timeout.BeforeRequestAsync(context, parameter);

            Assert.True(context.RequestMessage.Timeout == TimeSpan.FromMilliseconds(5000));
        }
    }
}
