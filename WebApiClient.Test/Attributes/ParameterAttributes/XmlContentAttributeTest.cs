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
    public class XmlContentAttributeTest
    {
        public interface IMyApi : IDisposable
        {
            ITask<HttpResponseMessage> PostAsync(object content);
        }

        public class Model
        {
            public string name { get; set; }

            public DateTime birthDay { get; set; }
        }

        [Fact]
        public async Task BeforeRequestAsyncTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync")));

            context.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.RequestMessage.Method = HttpMethod.Post; 
 
            var parameter = context.ApiActionDescriptor.Parameters[0].Clone(new Model
            {
                name = "laojiu",
                birthDay = DateTime.Parse("2010-10-10")
            });

            var attr = new XmlContentAttribute();
            await ((IApiParameterAttribute)attr).BeforeRequestAsync(context, parameter);

            var body = await context.RequestMessage.Content.ReadAsStringAsync();
            var target = context.HttpApiConfig.XmlFormatter.Serialize(parameter.Value, Encoding.UTF8);
            Assert.True(body == target);
        }
    }
}

