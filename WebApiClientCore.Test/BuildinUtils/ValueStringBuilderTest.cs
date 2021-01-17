using Xunit;

namespace WebApiClientCore.Test.BuildinUtils
{
    public class ValueStringBuilderTest
    {
        [Fact]
        public void AppendTest()
        {
            var builder = new ValueStringBuilder(stackalloc char[1]);
            builder.Append('a');
            builder.Append("bc");
            builder.Append('d');
            var str = builder.ToString();

            Assert.Equal("abcd", str);
        }
    }
}
