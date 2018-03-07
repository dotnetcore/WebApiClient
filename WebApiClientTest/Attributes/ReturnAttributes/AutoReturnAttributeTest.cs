using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using WebApiClient.Interfaces;
using Xunit;

namespace WebApiClientTest.Attributes.HttpActionAttributes
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
            var context = new ApiActionContext
            {
                HttpApiConfig = new HttpApiConfig(),
                RequestMessage = new HttpApiRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://www.webapi.com/")
                },
                ResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK),
                ApiActionDescriptor = ApiDescriptorCache
                .GetApiActionDescriptor(typeof(IMyApi)
                .GetMethod("HttpResponseMessageAsync"))
            };
            context.ResponseMessage.Content = new StringContent("laojiu");

            var attr = new AutoReturnAttribute();
            var result = await ((IApiReturnAttribute)attr).GetTaskResult(context);
            Assert.True(result is HttpResponseMessage);
        }

        [Fact]
        public async Task StringResultTest()
        {
            var context = new ApiActionContext
            {
                HttpApiConfig = new HttpApiConfig(),
                RequestMessage = new HttpApiRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://www.webapi.com/")
                },
                ResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK),
                ApiActionDescriptor = ApiDescriptorCache
                .GetApiActionDescriptor(typeof(IMyApi)
                .GetMethod("StringAsync"))
            };
            context.ResponseMessage.Content = new StringContent("laojiu");

            var attr = new AutoReturnAttribute();
            var result = await ((IApiReturnAttribute)attr).GetTaskResult(context);
            Assert.True(result?.ToString() == "laojiu");
        }

        [Fact]
        public async Task ByteArrayResultTest()
        {
            var context = new ApiActionContext
            {
                HttpApiConfig = new HttpApiConfig(),
                RequestMessage = new HttpApiRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://www.webapi.com/")
                },
                ResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK),
                ApiActionDescriptor = ApiDescriptorCache
                .GetApiActionDescriptor(typeof(IMyApi)
                .GetMethod("ByteArrayAsync"))
            };
            context.ResponseMessage.Content = new StringContent("laojiu", Encoding.UTF8);

            var attr = new AutoReturnAttribute();
            var result = await ((IApiReturnAttribute)attr).GetTaskResult(context);
            var text = Encoding.UTF8.GetString((byte[])result);
            Assert.True(text == "laojiu");
        }

        [Fact]
        public async Task JsonResultTest()
        {
            var context = new ApiActionContext
            {
                HttpApiConfig = new HttpApiConfig(),
                RequestMessage = new HttpApiRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://www.webapi.com/")
                },
                ResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK),
                ApiActionDescriptor = ApiDescriptorCache
                .GetApiActionDescriptor(typeof(IMyApi)
                .GetMethod("JsonXmlAsync"))
            };

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
            var context = new ApiActionContext
            {
                HttpApiConfig = new HttpApiConfig(),
                RequestMessage = new HttpApiRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://www.webapi.com/")
                },
                ResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK),
                ApiActionDescriptor = ApiDescriptorCache
                .GetApiActionDescriptor(typeof(IMyApi)
                .GetMethod("JsonXmlAsync"))
            };

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
            var context = new ApiActionContext
            {
                HttpApiConfig = new HttpApiConfig(),
                RequestMessage = new HttpApiRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://www.webapi.com/")
                },
                ResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound),
                ApiActionDescriptor = ApiDescriptorCache
                .GetApiActionDescriptor(typeof(IMyApi)
                .GetMethod("JsonXmlAsync"))
            };

            var model = new Model();
            var xml = context.HttpApiConfig.XmlFormatter.Serialize(model, Encoding.UTF8);
            context.ResponseMessage.Content = new StringContent(xml, Encoding.UTF8, "application/xml");

            var attr = new AutoReturnAttribute() { EnsureSuccessStatusCode = true };
            await Assert.ThrowsAsync<HttpFailureStatusException>(() => ((IApiReturnAttribute)attr).GetTaskResult(context));
        }
    }
}