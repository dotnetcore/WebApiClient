using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Contexts;
using WebApiClient.Parameterables;
using Xunit;

namespace WebApiClient.Test.Parameterables
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
                ApiActionDescriptor = ApiActionDescriptor.Create(typeof(IMyApi).GetMethod("PostAsync"))
            };

            var parameter = context.ApiActionDescriptor.Parameters[0];
            IApiParameterable timeout = new Timeout(50);
            await timeout.BeforeRequestAsync(context, parameter);

            await Task.Delay(100);
            var canceled = context.CancellationTokens[0].IsCancellationRequested;
            Assert.True(canceled);
        }
    }
}
