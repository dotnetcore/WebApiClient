using Xunit;

namespace WebApiClientCore.Test.ResponseCaches
{
    public class RequestHeaderTest
    {
        [Fact]
        public void GetNameTest()
        {
            var h = RequestHeader.GetName(HttpRequestHeader.Accept);
            Assert.True(h == "Accept");

            h = RequestHeader.GetName(HttpRequestHeader.AcceptCharset);
            Assert.True(h == "Accept-Charset");
        }
    }
}