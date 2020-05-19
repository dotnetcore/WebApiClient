using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using WebApiClientCore.Exceptions;
using Xunit;

namespace WebApiClientCore.Test.Attributes.ReturnAttributes
{
    public class JsonXmlModelResultAttributeTest
    {
        [Fact]
        public async Task JsonResultTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("JsonXmlAsync"));
            var context = new TestRequestContext(apiAction, "laojiu");
            var responseContext = new ApiResponseContext(context);

            context.HttpContext.RequestMessage.Method = HttpMethod.Post;

            var model = new TestModel();
            var json = context.HttpContext.Services.GetRequiredService<IJsonFormatter>().Serialize(model, null);
            context.HttpContext.ResponseMessage.Content = new JsonContent(json);

            var attr = new JsonResultAttribute();
            await attr.OnResponseAsync(responseContext, () => Task.CompletedTask);
            var result = responseContext.Result as TestModel;
            Assert.True(model.Name == result.Name && model.Age == result.Age);
        }

        [Fact]
        public async Task XmlResultTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("JsonXmlAsync"));
            var context = new TestRequestContext(apiAction, "laojiu");
            var responseContext = new ApiResponseContext(context);

            context.HttpContext.RequestMessage.Method = HttpMethod.Post;

            var model = new TestModel();
            var xml = context.HttpContext.Services.GetRequiredService<IXmlFormatter>().Serialize(model, Encoding.UTF8);
            context.HttpContext.ResponseMessage.Content = new XmlContent(xml, Encoding.UTF8);

            var attr = new XmlResultAttribute();
            await attr.OnResponseAsync(responseContext, () => Task.CompletedTask);
            var result = responseContext.Result as TestModel;
            Assert.True(model.Name == result.Name && model.Age == result.Age);
        }


        [Fact]
        public async Task EnsureSuccessStatusCodeTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("JsonXmlAsync"));
            var context = new TestRequestContext(apiAction, "laojiu");
            var responseContext = new ApiResponseContext(context);

            context.HttpContext.RequestMessage.Method = HttpMethod.Post;
            context.HttpContext.ResponseMessage.StatusCode = System.Net.HttpStatusCode.InternalServerError;

            var attr = new JsonResultAttribute() { EnsureSuccessStatusCode = true };
            await attr.OnResponseAsync(responseContext, () => Task.CompletedTask);

            Assert.IsType<HttpStatusFailureException>(responseContext.Exception);
        }
    }
}
