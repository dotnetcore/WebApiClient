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
            [IgnoreSerialized]
            public DateTime Birthday { get; set; }
        }


        [Fact]
        public void IgnoreSerializedTest()
        {
            var member = typeof(MyClass).GetProperty("Birthday");

            var annotations = Annotations.GetAnnotations(member, FormatScope.JsonFormat);
            Assert.True(annotations.IgnoreSerialized);

            annotations = Annotations.GetAnnotations(member, FormatScope.KeyValueFormat);
            Assert.True(annotations.IgnoreSerialized);
        }
    }
}
