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
    public class PathQueryAttributeTest
    {
        [Fact]
        public async Task OnRequestAsyncTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, new
            {
                name = "laojiu",
                birthDay = DateTime.Parse("2010-10-10")
            });

            context.HttpContext.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;

            var attr = new PathQueryAttribute();
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));

            var birthday = context.HttpContext.ServiceProvider.GetService<IKeyValueSerializer>().Serialize("time", DateTime.Parse("2010-10-10"), null)[0].Value;
            var target = new Uri("http://www.webapi.com?name=laojiu&birthDay=" + Uri.EscapeDataString(birthday));
            Assert.True(context.HttpContext.RequestMessage.RequestUri == target);
        }
    }
}
