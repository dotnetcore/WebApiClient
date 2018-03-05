using System;
using System.Linq;
using WebApiClient.Defaults.KeyValueFormats;
using WebApiClient.Defaults.KeyValueFormats.Converters;
using Xunit;

namespace WebApiClientTest.Defaults.KeyValueFormats
{
    public class PropertiesConverterTest
    {
        [Fact]
        public void InvokeTest()
        {
            var first = ConverterMiddleware.Link(new PropertiesConverter());
            var model = new Model { Age = 18, Name = "laojiu" };
            var context = new ConvertContext("name", model, 0, new WebApiClient.FormatOptions { UseCamelCase = true });
            var kvs = first.Invoke(context)
                .ToDictionary(item => item.Key, item => item.Value);

            Assert.True(kvs["name"] == "laojiu");
            Assert.True(kvs["age"] == "18");
            Assert.True(kvs["email"] == null);
            Assert.False(kvs.ContainsKey("birthday"));
        }

        class Model : FormatModel
        {
            public string Email { get; set; }

            public DateTime Birthday
            {
                set
                {
                }
            }
        }
    }
}