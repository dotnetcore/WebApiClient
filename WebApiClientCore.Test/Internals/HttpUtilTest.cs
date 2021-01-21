using System.Text;
using WebApiClientCore.Internals;
using Xunit;

namespace WebApiClientCore.Test.Internals
{
    public class HttpUtilTest
    {
        [Fact]
        public void UrlEncodeTest()
        {
            var str = "c++!=c#，没错！";
            var e1 = HttpUtil.UrlEncode(str, Encoding.UTF8);
            var e2 = System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
            Assert.Equal(e1, e2);

            str = "abc123";
            e1 = HttpUtil.UrlEncode(str, Encoding.UTF8);
            e2 = System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
            Assert.Equal(e1, e2);
        }

        [Fact]
        public void UrlEncode_BufferWriterTest()
        {
            var str = "c++!=c#，没错！";
            using var writer = new RecyclableBufferWriter<byte>();
            HttpUtil.UrlEncode(str, writer);
            var e1 = Encoding.UTF8.GetString(writer.WrittenSpan);
            var e2 = System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
            Assert.Equal(e1, e2);

            str = "abc123";
            writer.Clear();
            HttpUtil.UrlEncode(str, writer);
            e1 = Encoding.UTF8.GetString(writer.WrittenSpan);
            e2 = System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
            Assert.Equal(e1, e2);
        }
    }
}
