using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;


namespace WebApiClient.Test.Attributes
{
    public class MulitpartTextAttributeTest
    {
        private string get(string name, string value)
        {
            return $@"Content-Disposition: form-data; name=""{name}""

{HttpUtility.UrlEncode(value, Encoding.UTF8)}";
        }

        [Fact]
        public async Task IApiParameterAttributeTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: ApiActionDescriptor.Create(typeof(IMyApi).GetMethod("PostAsync")));

            context.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.RequestMessage.Method = HttpMethod.Post; 

            var parameter = context.ApiActionDescriptor.Parameters[0].Clone("laojiu");

            IApiParameterAttribute attr = new MulitpartTextAttribute();
            await attr.BeforeRequestAsync(context, parameter);
            var body = await context.RequestMessage.Content.ReadAsStringAsync();
            Assert.Contains(get("name", "laojiu"), body);

            // IgnoreWhenNull Test
            parameter = parameter.Clone(null);
            ((MulitpartTextAttribute)attr).IgnoreWhenNull = true;
            await attr.BeforeRequestAsync(context, parameter);
            body = await context.RequestMessage.Content.ReadAsStringAsync();
            Assert.DoesNotContain("age", body);
        }

        [Fact]
        public async Task IApiActionAttributeTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: ApiActionDescriptor.Create(typeof(IMyApi).GetMethod("PostAsync")));

            context.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.RequestMessage.Method = HttpMethod.Post; 

            var attr = new MulitpartTextAttribute("name", "laojiu");
            await attr.BeforeRequestAsync(context);
            var body = await context.RequestMessage.Content.ReadAsStringAsync();
            Assert.Contains(get("name", "laojiu"), body);


            // IgnoreWhenNull Test
            var attr2 = new MulitpartTextAttribute("age", null)
            {
                IgnoreWhenNull = true
            };
            await attr2.BeforeRequestAsync(context);
            body = await context.RequestMessage.Content.ReadAsStringAsync();
            Assert.DoesNotContain("age", body);
        }
    }
}
