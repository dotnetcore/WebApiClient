using System;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using Xunit;

namespace WebApiClientCore.Test.Attributes
{
    public class TagsAttributeTest
    {
        [Fact]
        public async Task OnRequestAsync()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, "laojiu");

            var attr = new TagsAttribute("key", "laojiu");
            await attr.OnRequestAsync(context);

            Assert.Equal("laojiu", context.Tags.Get("key").As<string>());
        }

        [Fact]
        public async Task OnRequestAsync_Parameter()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, "laojiu");
            var parameterContext = new ApiParameterContext(context, 0);

            var attr = new TagsAttribute("key");
            await attr.OnRequestAsync(parameterContext, () => Task.CompletedTask);
            Assert.Equal("laojiu", context.Tags.Get("key").As<string>());
        }

    }
}
