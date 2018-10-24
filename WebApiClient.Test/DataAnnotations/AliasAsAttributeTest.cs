using System;
using System.Linq;
using WebApiClient;
using WebApiClient.DataAnnotations;
using Xunit;

namespace WebApiClient.Test.DataAnnotations
{
    public class AliasAsAttributeTest
    {
        class MyClass
        {
            [AliasAs("MyName")]
            public string Name { get; set; }
        }

        [Fact]
        public void AliasNameTest()
        {
            var member = typeof(MyClass).GetProperty("Name");

            var annotations = Annotations.GetAnnotations(member, FormatScope.JsonFormat);
            Assert.Equal("MyName", annotations.AliasName);

            annotations = Annotations.GetAnnotations(member, FormatScope.KeyValueFormat);
            Assert.Equal("MyName", annotations.AliasName);
        }
    }
}
