using System;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using Xunit;

namespace WebApiClientCore.Test.Attributes.ParameterAttributes
{
    public class UriAttributeTest
    {
        [Fact]
        public async Task OnRequestAsyncRelativeTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, "http://www.baidu.com");

            var attr = new UriAttribute();
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.baidu.com"));

            context.Arguments[0] = "/login";
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.baidu.com/login"));

            context.Arguments[0] = "/login/login2";
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.baidu.com/login/login2"));

            context.Arguments[0] = "login3";
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.baidu.com/login/login3"));
        }


        [Fact]
        public async Task OnRequestAsyncAbsoluteTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, "/login");

            var attr = new UriAttribute();
            context.Arguments[0] = "http://www.baidu.com/login";
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.baidu.com/login"));

            context.Arguments[0] = "http://www.baidu.com/login/login2";
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.baidu.com/login/login2"));
        }

        [Fact]
        public async Task OnRequestAsyncAbsolute2Test()
        {
            var apiAction = new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, "/login");
           
            var attr = new UriAttribute();
            context.Arguments[0] = "http://www.baidu.com";
            context.HttpContext.RequestMessage.RequestUri = new Uri("http://www.abc.com/login");
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.baidu.com/login"));

            context.Arguments[0] = "http://www.baidu.com";
            context.HttpContext.RequestMessage.RequestUri = new Uri("http://www.abc.com/login/login2");
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.baidu.com/login/login2"));
        }
    }
}
