using System;
using Xunit;

namespace WebApiClientCore.Test.BuildinExtensions
{
    public class UriExtensionsTest
    {
        [Fact]
        public void AddQueryTest()
        {
            var uri = new Uri("http://www.webapiclient.com");
            uri.Replace("a", "a", out var replaced);
            Assert.False(replaced);
            uri = uri.AddQuery("a", "a");
            Assert.True(uri.ToString() == "http://www.webapiclient.com/?a=a");

            uri = new Uri("http://www.webapiclient.com/path");
            uri = uri.AddQuery("a", "a");
            Assert.True(uri.ToString() == "http://www.webapiclient.com/path?a=a");

            uri = new Uri("http://www.webapiclient.com/path/");
            uri = uri.AddQuery("a", "a");
            Assert.True(uri.ToString() == "http://www.webapiclient.com/path/?a=a");


            uri = new Uri("http://www.webapiclient.com/path/?");
            uri = uri.AddQuery("a", "a");
            Assert.True(uri.ToString() == "http://www.webapiclient.com/path/?a=a");

            uri = new Uri("http://www.webapiclient.com/path?x=1");
            uri = uri.AddQuery("a", "a");
            Assert.True(uri.ToString() == "http://www.webapiclient.com/path?x=1&a=a");


            uri = new Uri("http://www.webapiclient.com/path?x=1&");
            uri = uri.AddQuery("a", "a");
            Assert.True(uri.ToString() == "http://www.webapiclient.com/path?x=1&a=a");

            uri = new Uri("http://www.webapiclient.com/path?x=1&");
            uri = uri.AddQuery("a", "我");
            Assert.True(uri.ToString() == "http://www.webapiclient.com/path?x=1&a=我");


            uri = new Uri("http://www.webapiclient.com/path/?x=1&");
            uri = uri.AddQuery("a", "我");
            Assert.True(uri.ToString() == "http://www.webapiclient.com/path/?x=1&a=我");


            uri = new Uri("http://www.webapiclient.com/path/?x={x}&y={Y}&");
            uri = uri.AddQuery("a", "我");
            uri = uri.Replace("x", "你", out replaced);
            Assert.True(replaced);
            uri = uri.Replace("x", "你", out replaced);
            Assert.False(replaced);
            Assert.True(uri.ToString() == "http://www.webapiclient.com/path/?x=你&y={Y}&a=我");

            uri = new Uri("http://www.webapiclient.com");
            uri = uri.AddQuery("a", "我");
            uri = uri.AddQuery("b", "你");
            Assert.True(uri.ToString() == "http://www.webapiclient.com/?a=我&b=你");
        }
    }
}
