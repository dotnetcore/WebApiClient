
using System;
using System.Linq;
using WebApiClient;
using WebApiClient.DataAnnotations;
using Xunit;

namespace WebApiClient.Test.DataAnnotations
{
    public class IgnoreWhenNullAttributeTest
    {
        class MyClass
        {
            [IgnoreWhenNull(Scope = FormatScope.All)]
            public DateTime? Birthday { get; set; }

            public string Name { get; set; }
        }

        [Fact]
        public void Test()
        {
            var model = new MyClass();
            var json = HttpApiConfig.DefaultJsonFormatter.Serialize(model, null);
            Assert.DoesNotContain("Birthday", json);
            Assert.Contains("Name", json);

            var kvs = HttpApiConfig.DefaultKeyValueFormatter.Serialize("MyClass", model, null).ToArray();
            Assert.DoesNotContain(kvs, item => item.Key == "Birthday");
            Assert.Contains(kvs, item => item.Key == "Name");
        }
    }
}