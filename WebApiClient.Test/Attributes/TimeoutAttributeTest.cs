using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;


namespace WebApiClient.Test.Attributes
{
    public class TimeoutAttributeTest
    {
        [Fact]
        public async Task BeforeRequestAsyncTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync")));

            var attr = new TimeoutAttribute(50);
            await attr.BeforeRequestAsync(context);

            await Task.Delay(100);
            var canceled = context.CancellationTokens[0].IsCancellationRequested;
            Assert.True(canceled);
        }

        [Fact]
        public async Task BeforeRequestAsync_ParameterTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync")));

            IApiParameterAttribute attr = new TimeoutAttribute(50);

            var parameter = context.ApiActionDescriptor.Parameters[0].Clone(10);
            await attr.BeforeRequestAsync(context, parameter);

            await Task.Delay(20);
            var canceled = context.CancellationTokens[0].IsCancellationRequested;
            Assert.True(canceled);


            parameter = context.ApiActionDescriptor.Parameters[0].Clone(TimeSpan.FromMilliseconds(5));
            await attr.BeforeRequestAsync(context, parameter);

            await Task.Delay(10);
            canceled = context.CancellationTokens[0].IsCancellationRequested;
            Assert.True(canceled);
        }
    }
}
