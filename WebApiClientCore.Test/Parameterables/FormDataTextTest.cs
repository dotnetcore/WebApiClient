using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebApiClientCore.Parameterables;
using Xunit;

namespace WebApiClientCore.Test.Parameterables
{
    public class FormDataTextTest
    {
        [Fact]
        public async Task OnRequestAsync()
        {
            string get(string name, string value)
            {
                return $@"Content-Disposition: form-data; name=""{name}""

{HttpUtility.UrlEncode(value, Encoding.UTF8)}";
            }

            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, string.Empty);

            context.HttpContext.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;


            IApiParameterable mulitpartText = new FormDataText("laojiu");
            await mulitpartText.OnRequestAsync(new ApiParameterContext(context, 0));

            var body = await context.HttpContext.RequestMessage.Content.ReadAsStringAsync();
            Assert.Contains(get(name: "value", value: "laojiu"), body);
        }
    }
}
