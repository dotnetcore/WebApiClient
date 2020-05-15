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
        public async Task BeforeRequestAsync()
        {
            string get(string name, string value)
            {
                return $@"Content-Disposition: form-data; name=""{name}""

{HttpUtility.UrlEncode(value, Encoding.UTF8)}";
            }

            var context = new TestActionContext(                
                apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync")));

            context.HttpContext.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;
           

            var parameter = context.ApiAction.Parameters[0];
            IApiParameterable mulitpartText = new FormDataText("laojiu");
            await mulitpartText.BeforeRequestAsync(new ApiParameterContext(context, parameter));

            var body = await context.HttpContext.RequestMessage.Content.ReadAsStringAsync();
            Assert.Contains(get("name", "laojiu"), body);
        }
    }
}
