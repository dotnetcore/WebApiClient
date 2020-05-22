using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using Xunit;

namespace WebApiClientCore.Test.Attributes
{
    public class HeaderAttributeTest
    {
        [Fact]
        public async Task OnRequestAsync_Parameter()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, "laojiu");
            var parameterContext = new ApiParameterContext(context, 0);

            var attr = new HeaderAttribute("MyHeader");
            await attr.OnRequestAsync(parameterContext);

            context.HttpContext.RequestMessage.Headers.TryGetValues("MyHeader", out IEnumerable<string> values);
            Assert.Equal("laojiu", values.First());
        }

        [Fact]
        public async Task OnRequestAsync()
        {
            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, string.Empty);

            var attr = new HeaderAttribute("MyHeader", "laojiu");
            await attr.OnRequestAsync(context);

            context.HttpContext.RequestMessage.Headers.TryGetValues("MyHeader", out IEnumerable<string> values);
            Assert.Equal("laojiu", values.First());
        }
    }
}

