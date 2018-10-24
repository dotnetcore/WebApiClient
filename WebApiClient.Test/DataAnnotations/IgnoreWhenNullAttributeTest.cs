
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
            [IgnoreWhenNull]
            public DateTime? Birthday { get; set; }

            public string Name { get; set; }
        }

        [Fact]
        public void IgnoreWhenNullTest()
        {
            var birthday = typeof(MyClass).GetProperty("Birthday");
            var name = typeof(MyClass).GetProperty("Name");

            var annotations = Annotations.GetAnnotations(birthday, FormatScope.JsonFormat);
            Assert.False(annotations.IgnoreWhenNull);

            annotations = Annotations.GetAnnotations(birthday, FormatScope.KeyValueFormat);
            Assert.True(annotations.IgnoreWhenNull);

            annotations = Annotations.GetAnnotations(name, FormatScope.KeyValueFormat);
            Assert.False(annotations.IgnoreWhenNull);
        }
    }
}