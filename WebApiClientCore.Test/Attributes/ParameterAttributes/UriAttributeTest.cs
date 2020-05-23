using System;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using Xunit;

namespace WebApiClientCore.Test.Attributes.ParameterAttributes
{
    public class UriAttributeTest
    {
        [Fact]
        public async Task OnRequestAsyncTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, "http://www.baidu.com");

            var attr = new UriAttribute();
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.baidu.com"));

            context.Arguments[0] = "/login";
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));

            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.baidu.com/login"));
        }
    }
}
