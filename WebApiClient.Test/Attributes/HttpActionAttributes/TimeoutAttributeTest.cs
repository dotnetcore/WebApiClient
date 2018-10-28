using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;


namespace WebApiClient.Test.Attributes.HttpActionAttributes
{
    public class TimeoutAttributeTest
    {
        [Fact]
        public async Task BeforeRequestAsyncTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: ApiActionDescriptor.Create(typeof(IMyApi).GetMethod("PostAsync")));

            var attr = new TimeoutAttribute(50);
            await attr.BeforeRequestAsync(context);

            await Task.Delay(100);
            var canceled = context.CancellationTokens[0].IsCancellationRequested;
            Assert.True(canceled);
        }
    }
}
