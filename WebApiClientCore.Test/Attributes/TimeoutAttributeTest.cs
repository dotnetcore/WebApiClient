using System;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using WebApiClientCore.Exceptions;
using Xunit;


namespace WebApiClientCore.Test.Attributes
{
    public class TimeoutAttributeTest
    {
        [Fact]
        public async Task OnRequestAsync()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, 10);

            var attr = new TimeoutAttribute(50);
            await attr.OnRequestAsync(context);

            await Task.Delay(100);
            var canceled = context.HttpContext.CancellationTokens[0].IsCancellationRequested;
            Assert.True(canceled);
        }

        [Fact]
        public async Task OnRequestAsync_Parameter_Double_Test()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, 1);

            var attr = new TimeoutAttribute();
            var parameterContext = new ApiParameterContext(context, 0);
            await attr.OnRequestAsync(parameterContext);

            await Task.Delay(20);
            var canceled = context.HttpContext.CancellationTokens[0].IsCancellationRequested;
            Assert.True(canceled);
        }

        [Fact]
        public async Task OnRequestAsync_Parameter_Timespan_Test()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, 5);

            var attr = new TimeoutAttribute();

            var parameterContext = new ApiParameterContext(context, 0);
            await attr.OnRequestAsync(parameterContext);

            await Task.Delay(20);
            var canceled = context.HttpContext.CancellationTokens[0].IsCancellationRequested;
            Assert.True(canceled);

            context.Arguments[0] = Guid.NewGuid();

            await Assert.ThrowsAsync<ApiInvalidConfigException>(()
                => attr.OnRequestAsync(parameterContext));

            context.Arguments[0] = null;

            await attr.OnRequestAsync(parameterContext);
            Assert.True(context.HttpContext.CancellationTokens.Count == 1);
        }
    }
}
