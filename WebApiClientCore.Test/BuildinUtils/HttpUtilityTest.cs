using System.Text;
using Xunit;

namespace WebApiClientCore.Test.BuildinUtils
{
    public class HttpUtilityTest
    {
        [Fact]
        public void UrlEncodeTest()
        {
            var str = "c++!=c#，没错！";
            var e1 = HttpUtility.UrlEncode(str, Encoding.UTF8);
            var e2 = System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
            Assert.Equal(e1, e2);

            str = "abc123";
            e1 = HttpUtility.UrlEncode(str, Encoding.UTF8);
            e2 = System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
            Assert.Equal(e1, e2);
        }

        [Fact]
        public void UrlEncode_BufferWriterTest()
        {
            var str = "c++!=c#，没错！";
            using var writer = new BufferWriter<byte>();
            HttpUtility.UrlEncode(str, writer);
            var e1 = Encoding.UTF8.GetString(writer.GetWrittenSpan());
            var e2 = System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
            Assert.Equal(e1, e2);

            str = "abc123";
            writer.Clear();
            HttpUtility.UrlEncode(str, writer);
            e1 = Encoding.UTF8.GetString(writer.GetWrittenSpan());
            e2 = System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
            Assert.Equal(e1, e2);
        }
    }
}
