using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;

namespace WebApiClientTest.Attributes.HttpActionAttributes
{
    public class PathQueryAttributeTest
    {
        public interface IMyApi : IDisposable
        {
            ITask<HttpResponseMessage> PostAsync(object content);
        }

        [Fact]
        public async Task BeforeRequestAsyncTest()
        {
            var context = new ApiActionContext
            {
                HttpApiConfig = new HttpApiConfig(),
                RequestMessage = new HttpApiRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://www.webapi.com/")
                },
                ApiActionDescriptor = ApiDescriptorCache.GetApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"))
            };

            var parameter = context.ApiActionDescriptor.Parameters[0];
            parameter.Value = new
            {
                name = "laojiu",
                birthDay = DateTime.Parse("2010-10-10")
            };

            var attr = new PathQueryAttribute();
            await ((IApiParameterAttribute)attr).BeforeRequestAsync(context, parameter);

            var birthday = context.HttpApiConfig.FormatOptions.CloneChange(attr.DateTimeFormat).FormatDateTime(DateTime.Parse("2010-10-10"));
            var target = new Uri("http://www.webapi.com?name=laojiu&birthDay=" + HttpUtility.UrlEncode(birthday, Encoding.GetEncoding(attr.Encoding)));
            Assert.True(context.RequestMessage.RequestUri == target);
        }
    }
}

