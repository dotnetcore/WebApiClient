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
    public class TagsAttributeTest
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

            IApiParameterAttribute attr = new TagsAttribute("key");
            await attr.BeforeRequestAsync(context, parameter);
            Assert.Equal("laojiu", context.Tags.Get("key").As<string>());
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

            var attr = new TagsAttribute("key", "laojiu");
            await attr.BeforeRequestAsync(context);

            Assert.Equal("laojiu", context.Tags.Get("key").As<string>());
        }
    }
}
