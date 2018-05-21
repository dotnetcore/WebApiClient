using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;

namespace WebApiClient.Test.Attributes
{
    public class HeaderAttributeTest
    {
        [Fact]
        public async Task IApiParameterAttributeTest()
        {
            var context = new ApiActionContext
            {
                RequestMessage = new HttpApiRequestMessage
                {
                    RequestUri = new Uri("http://www.mywebapi.com"),
                    Method = HttpMethod.Post
                },
                ApiActionDescriptor = ApiDescriptorCache.GetApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"))
            };

            var parameter = context.ApiActionDescriptor.Parameters[0];
            parameter.Value = "laojiu";

            IApiParameterAttribute attr = new HeaderAttribute("MyHeader");
            await attr.BeforeRequestAsync(context, parameter);

            context.RequestMessage.Headers.TryGetValues("MyHeader", out IEnumerable<string> values);
            Assert.Equal("laojiu", values.First());
        }

        [Fact]
        public async Task IApiActionAttributeTest()
        {
            var context = new ApiActionContext
            {
                RequestMessage = new HttpApiRequestMessage
                {
                    RequestUri = new Uri("http://www.mywebapi.com"),
                    Method = HttpMethod.Post
                },
                ApiActionDescriptor = ApiDescriptorCache.GetApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"))
            };

            var attr = new HeaderAttribute("MyHeader", "laojiu");
            await attr.BeforeRequestAsync(context);

            context.RequestMessage.Headers.TryGetValues("MyHeader", out IEnumerable<string> values);
            Assert.Equal("laojiu", values.First());
        }
    }
}

