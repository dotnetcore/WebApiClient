using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;

namespace WebApiClient.Test.Attributes.HttpActionAttributes
{
    public class AutoReturnAttributeTest
    {
        public interface IMyApi : IDisposable
        {
            ITask<HttpResponseMessage> HttpResponseMessageAsync();

            ITask<string> StringAsync();

            ITask<byte[]> ByteArrayAsync();

            ITask<Model> JsonXmlAsync();
        }

        public class Model
        {
            public string Name { get; set; } = "laojiu";
            public int Age { get; set; } = 18;
        }

        [Fact]
        public async Task HttpResponseMessageResultTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("HttpResponseMessageAsync")));

            context.RequestMessage.RequestUri = new Uri("http://www.mywebapi.com");
            context.RequestMessage.Method = HttpMethod.Post; 
            context.ResponseMessage.Content = new StringContent("laojiu", Encoding.UTF8);


            var attr = new AutoReturnAttribute();
            var result = await ((IApiReturnAttribute)attr).GetTaskResult(context);
            Assert.True(result is HttpResponseMessage);
        }

        [Fact]
        public async Task StringResultTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("StringAsync")));

            context.RequestMessage.RequestUri = new Uri("http://www.mywebapi.com");
            context.RequestMessage.Method = HttpMethod.Post; 
            context.ResponseMessage.Content = new StringContent("laojiu");

            var attr = new AutoReturnAttribute();
            var result = await ((IApiReturnAttribute)attr).GetTaskResult(context);
            Assert.True(result?.ToString() == "laojiu");
        }

        [Fact]
        public async Task ByteArrayResultTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("ByteArrayAsync")));

            context.RequestMessage.RequestUri = new Uri("http://www.mywebapi.com");
            context.RequestMessage.Method = HttpMethod.Post;           
            context.ResponseMessage.Content = new StringContent("laojiu", Encoding.UTF8);

            var attr = new AutoReturnAttribute();
            var result = await ((IApiReturnAttribute)attr).GetTaskResult(context);
            var text = Encoding.UTF8.GetString((byte[])result);
            Assert.True(text == "laojiu");
        }

        [Fact]
        public async Task JsonResultTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("JsonXmlAsync")));

            context.RequestMessage.RequestUri = new Uri("http://www.mywebapi.com");
            context.RequestMessage.Method = HttpMethod.Post; 


            var model = new Model();
            var json = context.HttpApiConfig.JsonFormatter.Serialize(model, context.HttpApiConfig.FormatOptions);
            context.ResponseMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var attr = new AutoReturnAttribute();
            var result = await ((IApiReturnAttribute)attr).GetTaskResult(context) as Model;
            Assert.True(model.Name == result.Name && model.Age == result.Age);
        }

        [Fact]
        public async Task XmlResultTest()
        {
            var context = new TestActionContext(
              httpApi: null,
              httpApiConfig: new HttpApiConfig(),
              apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("JsonXmlAsync")));

            context.RequestMessage.RequestUri = new Uri("http://www.mywebapi.com");
            context.RequestMessage.Method = HttpMethod.Post;
            

            var model = new Model();
            var xml = context.HttpApiConfig.XmlFormatter.Serialize(model, Encoding.UTF8);
            context.ResponseMessage.Content = new StringContent(xml, Encoding.UTF8, "application/xml");

            var attr = new AutoReturnAttribute();
            var result = await ((IApiReturnAttribute)attr).GetTaskResult(context) as Model;
            Assert.True(model.Name == result.Name && model.Age == result.Age);
        }

        [Fact]
        public async Task EnsureSuccessStatusCodeTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("JsonXmlAsync")));

            context.RequestMessage.RequestUri = new Uri("http://www.mywebapi.com");
            context.RequestMessage.Method = HttpMethod.Post;
            context.ResponseMessage.StatusCode = System.Net.HttpStatusCode.InternalServerError;

            var model = new Model();
            var xml = context.HttpApiConfig.XmlFormatter.Serialize(model, Encoding.UTF8);
            context.ResponseMessage.Content = new StringContent(xml, Encoding.UTF8, "application/xml");

            var attr = new AutoReturnAttribute() { EnsureSuccessStatusCode = true };
            await Assert.ThrowsAsync<HttpStatusFailureException>(() => ((IApiReturnAttribute)attr).GetTaskResult(context));
        }
    }
}