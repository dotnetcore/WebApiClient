using System;
using System.Linq;
using WebApiClient;
using WebApiClient.DataAnnotations;
using Xunit;

namespace WebApiClientTest.DataAnnotations
{
    public class DateTimeFormatAttributeTest
    {
        class MyClass
        {
            [DateTimeFormat("yyyy年MM月")]
            public DateTime Birthday { get; set; }
        }

        [Fact]
        public void Test()
        {
            var model = new MyClass()
            {
                Birthday = DateTime.Parse("2000-1-1")
            };
            var json = HttpApiConfig.DefaultJsonFormatter.Serialize(model, null);
            Assert.Contains("2000年01月", json);

            var kvs = HttpApiConfig.DefaultKeyValueFormatter.Serialize("name", model, null).ToArray();
            Assert.Contains(kvs, item => item.Value == "2000年01月");
        }
    }
}
