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
    public class FormContentAttributeTest
    {
        public interface IMyApi : IDisposable
        {
            ITask<HttpResponseMessage> PostAsync(object content);
        }

        [Fact]
        public async Task BeforeRequestAsyncTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync")));

            context.RequestMessage.RequestUri = new Uri("http://www.mywebapi.com");
            context.RequestMessage.Method = HttpMethod.Post; 


            var parameter = context.ApiActionDescriptor.Parameters[0].Clone(new
            {
                name = "老 九",
                birthDay = DateTime.Parse("2010-10-10")
            });

            var attr = new FormContentAttribute();
            await ((IApiParameterAttribute)attr).BeforeRequestAsync(context, parameter);

            var body = await context.RequestMessage.Content.ReadAsStringAsync();
            var time = context.HttpApiConfig.FormatOptions.CloneChange(attr.DateTimeFormat).FormatDateTime(DateTime.Parse("2010-10-10"));
            var target = $"name={HttpUtility.UrlEncode("老 九", Encoding.UTF8)}&birthDay={HttpUtility.UrlEncode(time, Encoding.UTF8)}";
            Assert.True(body.ToUpper() == target.ToUpper());
        }
    }
}
