using System;
using System.Linq;
using WebApiClient;
using WebApiClient.DataAnnotations;
using Xunit;

namespace WebApiClient.Test.DataAnnotations
{
    public class IgnoreSerializedAttributeTest
    {
        class MyClass
        {
            [IgnoreSerialized(Scope = FormatScope.All)]
            public DateTime Birthday { get; set; }
        }

        [Fact]
        public void Test()
        {
            var model = new MyClass();
            var json = HttpApiConfig.DefaultJsonFormatter.Serialize(model, null);
            Assert.DoesNotContain("Birthday", json);

            var kvs = HttpApiConfig.DefaultKeyValueFormatter.Serialize("MyClass", model, null).ToArray();
            Assert.DoesNotContain(kvs, item => item.Key == "Birthday");
        }
    }
}
