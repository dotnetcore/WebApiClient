using System;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using Xunit;

namespace WebApiClientCore.Test.Attributes.ActionAttributes
{
    public class HttpHostAttributeTest
    {
        [Fact]
        public async Task OnRequestAsyncTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, string.Empty);

            Assert.Throws<ArgumentNullException>(() => new HttpHostAttribute(null));
            Assert.Throws<UriFormatException>(() => new HttpHostAttribute("/"));

            context.HttpContext.RequestMessage.RequestUri = null;
            var attr = new HttpHostAttribute("http://www.webapiclient.com");
            await attr.OnRequestAsync(context);
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.webapiclient.com"));
        }
    }
}
