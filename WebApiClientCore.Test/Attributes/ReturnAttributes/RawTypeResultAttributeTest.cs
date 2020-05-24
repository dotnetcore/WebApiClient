using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using Xunit;

namespace WebApiClientCore.Test.Attributes.ReturnAttributes
{
    public class RawTypeResultAttributeTest
    {
        [Fact]
        public async Task HttpResponseMessageResultTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("HttpResponseMessageAsync"));
            var context = new TestRequestContext(apiAction, "laojiu");
            var responseContext = new ApiResponseContext(context);

            context.HttpContext.RequestMessage.Method = HttpMethod.Post;
            context.HttpContext.ResponseMessage.Content = new StringContent("laojiu", Encoding.UTF8);

            var attr = new RawReturnAttribute();
            await attr.OnResponseAsync(responseContext);
            Assert.True(responseContext.Result is HttpResponseMessage);
        }

        [Fact]
        public async Task StringResultTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("StringAsync"));
            var context = new TestRequestContext(apiAction, "laojiu");
            var responseContext = new ApiResponseContext(context);

            context.HttpContext.RequestMessage.Method = HttpMethod.Post;
            context.HttpContext.ResponseMessage.Content = new StringContent("laojiu", Encoding.UTF8);

            var attr = new RawReturnAttribute();
            await attr.OnResponseAsync(responseContext);

            Assert.True(responseContext.Result?.ToString() == "laojiu");
        }

        [Fact]
        public async Task ByteArrayResultTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("ByteArrayAsync"));
            var context = new TestRequestContext(apiAction, "laojiu");
            var responseContext = new ApiResponseContext(context);

            context.HttpContext.RequestMessage.Method = HttpMethod.Post;
            context.HttpContext.ResponseMessage.Content = new StringContent("laojiu", Encoding.UTF8);

            var attr = new RawReturnAttribute();
            await attr.OnResponseAsync(responseContext);

            var text = Encoding.UTF8.GetString((byte[])responseContext.Result);
            Assert.True(text == "laojiu");
        }

    }
}