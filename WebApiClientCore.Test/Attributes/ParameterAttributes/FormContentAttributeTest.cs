using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebApiClientCore.Attributes;
using WebApiClientCore.Serialization;
using Xunit;

namespace WebApiClientCore.Test.Attributes.ParameterAttributes
{
    public class FormContentAttributeTest
    { 
        [Fact]
        public async Task OnRequestAsyncTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, new
            {
                name = "老 九",
                birthDay = DateTime.Parse("2010-10-10")
            });

            context.HttpContext.RequestMessage.RequestUri = new Uri("http://www.mywebapi.com");
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;

            var attr = new FormContentAttribute();
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));

            var body = await context.HttpContext.RequestMessage.Content.ReadAsStringAsync();
            var time = context.HttpContext.ServiceProvider.GetService<IKeyValueSerializer>().Serialize("time", DateTime.Parse("2010-10-10"), null);
            var target = $"name={HttpUtility.UrlEncode("老 九", Encoding.UTF8)}&birthDay={HttpUtility.UrlEncode(time[0].Value, Encoding.UTF8)}";
            Assert.True(body.ToUpper() == target.ToUpper());
        }
    }
}
