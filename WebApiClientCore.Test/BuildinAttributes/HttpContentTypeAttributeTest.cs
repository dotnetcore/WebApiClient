using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using Xunit;

namespace WebApiClientCore.Test.BuildinAttributes
{
    public class HttpContentTypeAttributeTest
    {
        public interface IMyApi
        {
            ITask<HttpResponseMessage> PostAsync(HttpContent content);
        }

        [Fact]
        public async Task OnRequestAsyncTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, new StringContent("laojiu"));

            context.HttpContext.RequestMessage.RequestUri = new Uri("http://www.mywebapi.com");
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;

            var attr = new HttpContentTypeAttribute();
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));

            var body = await context.HttpContext.RequestMessage.Content.ReadAsStringAsync();
            Assert.True(body == "laojiu");
        }
    }
}
