using System;
using System.Collections.Generic;
using System.Linq;
using WebApiClient.Defaults.KeyValueFormats;
using WebApiClient.Defaults.KeyValueFormats.Converters;
using Xunit;

namespace WebApiClientTest.Defaults.KeyValueFormats
{
    public class EnumerableConverterTest
    {
        [Fact]
        public void InvokeTest()
        {
            var first = ConverterMiddleware.Link(new SimpleTypeConverter(), new EnumerableConverter());
            var model = new object[] { "30", 30, 30m, 30d, 30f };
            var context = new ConvertContext("v", model, 0, null);
            var kvs = first.Invoke(context).ToArray();

            Assert.True(kvs.Length == model.Length);
            Assert.True(kvs.All(kv => kv.Key == "v" && kv.Value == "30"));
        }
    }
}