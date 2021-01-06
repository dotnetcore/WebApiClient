using Xunit;

namespace WebApiClientCore.Test.ResponseCaches
{
    public class MediaTypeTest
    {
        [Fact]
        public void IsMatchTest()
        {
            Assert.True(MediaType.IsMatch("a/a", "a/a"));
            Assert.True(MediaType.IsMatch("a/a", "a/a"));
            Assert.True(MediaType.IsMatch("a/a", "A/a"));
            Assert.True(MediaType.IsMatch("a/a", "A/A"));

            Assert.True(MediaType.IsMatch("a/*", "A/A"));
            Assert.True(MediaType.IsMatch("*/*", "A/A"));

            Assert.False(MediaType.IsMatch("a/b", "a/a"));
            Assert.False(MediaType.IsMatch("a/B", "A/a"));
            Assert.False(MediaType.IsMatch("a/b", "A/A"));

            Assert.False(MediaType.IsMatch("a/*", "b/A"));
            Assert.False(MediaType.IsMatch("a/*", null));
            Assert.False(MediaType.IsMatch("a/*", ""));
            Assert.False(MediaType.IsMatch("a", "a/a"));
        }

    }
}
