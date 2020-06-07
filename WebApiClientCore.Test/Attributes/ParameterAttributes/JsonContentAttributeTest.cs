using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using WebApiClientCore.Serialization;
using Xunit;

namespace WebApiClientCore.Test.Attributes.ParameterAttributes
{
    public class JsonContentAttributeTest
    {
        [Fact]
        public async Task BeforeRequestAsyncTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, new
            {
                name = "laojiu",
                birthDay = DateTime.Parse("2010-10-10")
            });

            context.HttpContext.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;

            var attr = new JsonContentAttribute() { CharSet = "utf-16" };
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));

            var body = await context.HttpContext.RequestMessage.Content.ReadAsUtf8ByteArrayAsync();

            var options = context.HttpContext.HttpApiOptions.JsonSerializeOptions;
            using var buffer = new BufferWriter<byte>();
            context.HttpContext.ServiceProvider.GetService<IJsonSerializer>().Serialize(buffer, context.Arguments[0], options);
            var target = buffer.GetWrittenSpan().ToArray();
            Assert.True(body.SequenceEqual(target));
        }
    }
}
