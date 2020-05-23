using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using Xunit;

namespace WebApiClientCore.Test.Attributes.ActionAttributes
{
    public class HttpOptionsAttributeTest
    {
        [Fact]
        public async Task OnRequestAsync()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, string.Empty);

            context.HttpContext.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;


            var attr = new HttpOptionsAttribute();
            await attr.OnRequestAsync(context);
            Assert.True(context.HttpContext.RequestMessage.Method == HttpMethod.Options);

            var attr2 = new HttpOptionsAttribute("/login");
            await attr2.OnRequestAsync(context);
            Assert.True(context.HttpContext.RequestMessage.Method == HttpMethod.Options);
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.webapi.com/login"));

            var attr3 = new HttpOptionsAttribute("http://www.baidu.com");
            await attr3.OnRequestAsync(context);
            Assert.True(context.HttpContext.RequestMessage.Method == HttpMethod.Options);
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.baidu.com"));
        }
    }
}
