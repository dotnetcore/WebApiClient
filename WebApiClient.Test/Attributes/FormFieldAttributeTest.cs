using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;


namespace WebApiClient.Test.Attributes
{
    public class FormFieldAttributeTest
    {
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
            IApiParameterAttribute attr = new FormFieldAttribute();
            await attr.BeforeRequestAsync(context, parameter);
            var body = await context.RequestMessage.Content.ReadAsStringAsync();
            Assert.Equal("name=laojiu", body);

            // IgnoreWhenNull Test
            parameter = parameter.Clone(null);
            ((FormFieldAttribute)attr).IgnoreWhenNull = true;
            await attr.BeforeRequestAsync(context, parameter);
            body = await context.RequestMessage.Content.ReadAsStringAsync();
            Assert.Equal("name=laojiu", body);
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
            
            var attr = new FormFieldAttribute("name", "laojiu");
            await attr.BeforeRequestAsync(context);
            var body = await context.RequestMessage.Content.ReadAsStringAsync();
            Assert.Equal("name=laojiu", body);


            // IgnoreWhenNull Test
            var attr2 = new FormFieldAttribute("age", null)
            {
                IgnoreWhenNull = true
            };
            await attr2.BeforeRequestAsync(context);
            body = await context.RequestMessage.Content.ReadAsStringAsync();
            Assert.Equal("name=laojiu", body);
        }
    }
}
