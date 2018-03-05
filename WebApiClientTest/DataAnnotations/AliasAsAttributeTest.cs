using System;
using System.Linq;
using WebApiClient;
using WebApiClient.DataAnnotations;
using Xunit;

namespace WebApiClientTest.DataAnnotations
{
    public class AliasAsAttributeTest
    {
        class MyClass
        {
            [AliasAs("MyName")]
            public string Name { get; set; }
        }

        [Fact]
        public void Test()
        {
            var model = new MyClass();
            var json = HttpApiConfig.DefaultJsonFormatter.Serialize(model, null);
            Assert.Contains("MyName", json);

            var kvs = HttpApiConfig.DefaultKeyValueFormatter.Serialize("name", model, null).ToArray();
            Assert.Contains(kvs, item => item.Key == "MyName");
        }
    }
}
