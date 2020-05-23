using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using Xunit;

namespace WebApiClientCore.Test.Attributes.ActionAttributes
{
    public class HttpDeleteAttributeTest
    {
        [Fact]
        public async Task OnRequestAsyncTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, string.Empty);

            context.HttpContext.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;

            var attr = new HttpDeleteAttribute();
            await attr.OnRequestAsync(context);
            Assert.True(context.HttpContext.RequestMessage.Method == HttpMethod.Delete);

            var attr2 = new HttpDeleteAttribute("/login");
            await attr2.OnRequestAsync(context);
            Assert.True(context.HttpContext.RequestMessage.Method == HttpMethod.Delete);
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.webapi.com/login"));

            var attr3 = new HttpDeleteAttribute("http://www.baidu.com");
            await attr3.OnRequestAsync(context);
            Assert.True(context.HttpContext.RequestMessage.Method == HttpMethod.Delete);
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.baidu.com"));
        }
    }
}
