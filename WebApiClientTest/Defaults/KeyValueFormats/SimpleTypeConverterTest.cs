using System;
using System.Linq;
using WebApiClient.Defaults.KeyValueFormats;
using WebApiClient.Defaults.KeyValueFormats.Converters;
using Xunit;

namespace WebApiClientTest.Defaults.KeyValueFormats
{
    public class SimpleTypeConverterTest
    {
        [Fact]
        public void InvokeTest()
        {
            var first = ConverterMiddleware.Link(new SimpleTypeConverter());

            var context = new ConvertContext("name", "laojiu", 0, null);
            var kvs = first.Invoke(context)
                .ToDictionary(item => item.Key, item => item.Value);
            Assert.True(kvs["name"] == "laojiu");

            context = new ConvertContext("age", 18, 0, null);
            kvs = first.Invoke(context)
              .ToDictionary(item => item.Key, item => item.Value);
            Assert.True(kvs["age"] == "18");

            context = new ConvertContext("Time", DateTime.Parse("2010-10-10 10:10:10"), 0, new WebApiClient.FormatOptions { DateTimeFormat = "yyyy-MM", UseCamelCase = true });
            kvs = first.Invoke(context)
              .ToDictionary(item => item.Key, item => item.Value);
            Assert.True(kvs["time"] == "2010-10");

            context = new ConvertContext("e", E.e, 0, null);
            kvs = first.Invoke(context)
              .ToDictionary(item => item.Key, item => item.Value);
            Assert.True(kvs["e"] == "1");

            context = new ConvertContext("f", "1.0", 0, null);
            kvs = first.Invoke(context)
              .ToDictionary(item => item.Key, item => item.Value);
            Assert.True(kvs["f"] == "1.0");

            context = new ConvertContext("v", Version.Parse("1.0"), 0, null);
            kvs = first.Invoke(context)
              .ToDictionary(item => item.Key, item => item.Value);
            Assert.True(kvs["v"] == "1.0");
        }

        public enum E
        {
            e = 1
        }
    }
}
