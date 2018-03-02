using System;
using Xunit;
using WebApiClient;

namespace WebApiClientTest
{
    public class TagsTest
    {
        [Fact]
        public void Test_All_Methods()
        {
            var tags = new Tags();
            tags.Id = "id";
            tags.Set("string", "a");
            tags.Set("int", 1);
            tags.Set("class", new TagsTest());

            Assert.True(tags.Id == "id");
            Assert.True(tags["string"].As<string>() == "a");
            Assert.True(tags.Get("int").AsInt32() == 1);
            Assert.True(tags.Get("class").As<TagsTest>() != null);

            tags.Remove("class");
            Assert.True(tags.Get("class").IsNull);
        }
    }
}
