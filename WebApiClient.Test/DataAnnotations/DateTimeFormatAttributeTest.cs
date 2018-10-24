using System;
using System.Linq;
using WebApiClient;
using WebApiClient.DataAnnotations;
using Xunit;

namespace WebApiClient.Test.DataAnnotations
{
    public class DateTimeFormatAttributeTest
    {
        class MyClass
        {
            [DateTimeFormat("yyyy年MM月")]
            public DateTime Birthday { get; set; }
        }

        [Fact]
        public void DateTimeFormatTest()
        {
            var member = typeof(MyClass).GetProperty("Birthday");

            var annotations = Annotations.GetAnnotations(member, FormatScope.JsonFormat);
            Assert.Equal("yyyy年MM月", annotations.DateTimeFormat);

            annotations = Annotations.GetAnnotations(member, FormatScope.KeyValueFormat);
            Assert.Equal("yyyy年MM月", annotations.DateTimeFormat);
        }
    }
}
