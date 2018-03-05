

using System;
using System.Collections.Generic;
using System.Linq;
using WebApiClient.Defaults.KeyValueFormats;
using WebApiClient.Defaults.KeyValueFormats.Converters;
using Xunit;

namespace WebApiClientTest.Defaults.KeyValueFormats
{
    public class KeyValuePairConverterTest
    {
        [Fact]
        public void InvokeTest()
        {
            var first = ConverterMiddleware.Link(new KeyValuePairConverter());
            var model = new KeyValuePair<string, object>("name", "laojiu");
            var context = new ConvertContext("model", model, 0, new WebApiClient.FormatOptions { UseCamelCase = true });
            var kvs = first.Invoke(context)
                .ToDictionary(item => item.Key, item => item.Value);

            Assert.True(kvs["name"] == "laojiu");
        }
    }
}