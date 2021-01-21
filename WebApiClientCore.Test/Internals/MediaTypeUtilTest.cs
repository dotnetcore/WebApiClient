using WebApiClientCore.Internals;
using Xunit;

namespace WebApiClientCore.Test.Internals
{
    public class MediaTypeUtilTest
    {
        [Fact]
        public void IsMatchTest()
        {
            Assert.True(MediaTypeUtil.IsMatch("a/a", "a/a"));
            Assert.True(MediaTypeUtil.IsMatch("a/a", "a/a"));
            Assert.True(MediaTypeUtil.IsMatch("a/a", "A/a"));
            Assert.True(MediaTypeUtil.IsMatch("a/a", "A/A"));

            Assert.True(MediaTypeUtil.IsMatch("a/*", "A/A"));
            Assert.True(MediaTypeUtil.IsMatch("*/*", "A/A"));

            Assert.False(MediaTypeUtil.IsMatch("a/b", "a/a"));
            Assert.False(MediaTypeUtil.IsMatch("a/B", "A/a"));
            Assert.False(MediaTypeUtil.IsMatch("a/b", "A/A"));

            Assert.False(MediaTypeUtil.IsMatch("a/*", "b/A"));
            Assert.False(MediaTypeUtil.IsMatch("a/*", null));
            Assert.False(MediaTypeUtil.IsMatch("a/*", ""));
            Assert.False(MediaTypeUtil.IsMatch("a", "a/a"));
        }

    }
}
