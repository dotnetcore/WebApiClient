using System;
using System.Collections.Generic;
using System.Text;
using WebApiClient;
using Xunit;

namespace WebApiClient.Test.Internal
{
    public class RequestHeaderTest
    {
        [Fact]
        public void Test()
        {
            var h = RequestHeader.GetName(HttpRequestHeader.Accept);
            Assert.True(h == "Accept");

            h = RequestHeader.GetName(HttpRequestHeader.AcceptCharset);
            Assert.True(h == "Accept-Charset");
        }
    }
}