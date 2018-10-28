using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Contexts;
using WebApiClient.Parameterables;
using Xunit;

namespace WebApiClient.Test.Parameterables
{
    public class MulitpartTextTest
    {
        [Fact]
        public async Task Test()
        {
            string get(string name, string value)
            {
                return $@"Content-Disposition: form-data; name=""{name}""

{HttpUtility.UrlEncode(value, Encoding.UTF8)}";
            }

            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: ApiActionDescriptor.Create(typeof(IMyApi).GetMethod("PostAsync")));

            context.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.RequestMessage.Method = HttpMethod.Post;
           

            var parameter = context.ApiActionDescriptor.Parameters[0];
            IApiParameterable mulitpartText = new MulitpartText("laojiu");
            await mulitpartText.BeforeRequestAsync(context, parameter);

            var body = await context.RequestMessage.Content.ReadAsStringAsync();
            Assert.Contains(get("name", "laojiu"), body);
        }
    }
}
