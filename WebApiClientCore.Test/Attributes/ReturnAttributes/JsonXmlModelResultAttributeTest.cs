using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using WebApiClientCore.Exceptions;
using WebApiClientCore.HttpContents;
using WebApiClientCore.Serialization;
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
            var jsonContent = new JsonContent();
            context.HttpContext.ResponseMessage.Content = jsonContent;
            context.HttpContext.ServiceProvider.GetRequiredService<IJsonSerializer>().Serialize(jsonContent, model, null);

            var attr = new JsonReturnAttribute();
            await attr.OnResponseAsync(responseContext);
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
            var xml = context.HttpContext.ServiceProvider.GetRequiredService<IXmlSerializer>().Serialize(model, null);
            context.HttpContext.ResponseMessage.Content = new XmlContent(xml, Encoding.UTF8);

            var attr = new XmlReturnAttribute();
            await attr.OnResponseAsync(responseContext);
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
            context.HttpContext.ResponseMessage.Content = new JsonContent();
            context.HttpContext.ResponseMessage.StatusCode = System.Net.HttpStatusCode.InternalServerError;

            await Assert.ThrowsAsync<ApiResponseStatusException>(async () =>
            {
                var attr = new JsonReturnAttribute() { EnsureSuccessStatusCode = true };
                await attr.OnResponseAsync(responseContext);
            });
        }

        [Fact]
        public void OrderIndexTest()
        {
            var attr = new JsonReturnAttribute();
            var attr2 = new JsonReturnAttribute(1d);
            var attr3 = new JsonReturnAttribute(0.5d);

            Assert.Equal(attr.OrderIndex, attr2.OrderIndex);
            Assert.True(attr.OrderIndex < attr3.OrderIndex);
        }
    }
}
