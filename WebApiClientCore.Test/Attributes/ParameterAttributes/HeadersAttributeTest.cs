using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using Xunit;

namespace WebApiClientCore.Test.Attributes.ParameterAttributes
{
    public class HeadersAttributeTest
    {

        [Fact]
        public async Task OnRequestAsyncTest()
        {
            var apiAction = new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, new
            {
                @class = 123,
                xx_yy = "xxyy"
            });

            var attr = new HeadersAttribute();
            context.HttpContext.RequestMessage.Headers.Clear();
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));         

            context.HttpContext.RequestMessage.Headers.TryGetValues("xx-yy", out IEnumerable<string> values);
            Assert.Equal("xxyy", values.FirstOrDefault());

            context.HttpContext.RequestMessage.Headers.TryGetValues("class", out IEnumerable<string> cValues);
            Assert.Equal("123", cValues.FirstOrDefault());


           
            attr.UnderlineToMinus = false;
            context.HttpContext.RequestMessage.Headers.Clear();
            await attr.OnRequestAsync(new ApiParameterContext(context, 0));

            context.HttpContext.RequestMessage.Headers.TryGetValues("xx_yy", out IEnumerable<string> values2);
            Assert.Equal("xxyy", values2.FirstOrDefault());
        }
    }
}
