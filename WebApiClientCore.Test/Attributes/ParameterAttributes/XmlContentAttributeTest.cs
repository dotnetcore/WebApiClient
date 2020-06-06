using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using WebApiClientCore.Serialization;
using Xunit;

namespace WebApiClientCore.Test.Attributes.ParameterAttributes
{
    public class XmlContentAttributeTest
    { 
        public class Model
        {
            public string name { get; set; }

            public DateTime birthDay { get; set; }
        }

        [Fact]
        public async Task OnRequestAsyncTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, new Model
            {
                name = "laojiu",
                birthDay = DateTime.Parse("2010-10-10")
            });

            context.HttpContext.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;
             
            var attr = new XmlContentAttribute();
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));

            var body = await context.HttpContext.RequestMessage.Content.ReadAsStringAsync();
            var target = context.HttpContext.ServiceProvider.GetService<IXmlSerializer>().Serialize(context.Arguments[0],null);
            Assert.True(body == target);
        }
    }
}
