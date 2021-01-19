using System;
using Xunit;

namespace WebApiClientCore.Test.BuildinUtils
{
    public class UriStringTest
    {
        [Fact]
        public void AddQueryTest()
        {
            var uriString = new UriString("http://www.webapiclient.com");
            uriString = uriString.AddQuery("a", "a");
            Assert.Equal("http://www.webapiclient.com/?a=a", uriString.ToString());

            uriString = new UriString("http://www.webapiclient.com/path");
            uriString = uriString.AddQuery("a", "a");
            Assert.Equal("http://www.webapiclient.com/path?a=a", uriString.ToString());

            uriString = new UriString("http://www.webapiclient.com/path/");
            uriString = uriString.AddQuery("a", "a");
            Assert.Equal("http://www.webapiclient.com/path/?a=a", uriString.ToString());


            uriString = new UriString("http://www.webapiclient.com/path/?");
            uriString = uriString.AddQuery("a", "a");
            Assert.Equal("http://www.webapiclient.com/path/?a=a", uriString.ToString());

            uriString = new UriString("http://www.webapiclient.com/path?x=1");
            uriString = uriString.AddQuery("a", "a");
            Assert.Equal("http://www.webapiclient.com/path?x=1&a=a", uriString.ToString());


            uriString = new UriString("http://www.webapiclient.com/path?x=1&");
            uriString = uriString.AddQuery("a", "a");
            Assert.Equal("http://www.webapiclient.com/path?x=1&a=a", uriString.ToString());

            uriString = new UriString("http://www.webapiclient.com/path?x=1&");
            uriString = uriString.AddQuery("a", "我");
            Assert.Equal($"http://www.webapiclient.com/path?x=1&a={Uri.EscapeDataString("我")}", uriString.ToString());


            uriString = new UriString("http://www.webapiclient.com/path/?x=1&");
            uriString = uriString.AddQuery("a", "我");
            Assert.True(uriString.ToString() == $"http://www.webapiclient.com/path/?x=1&a={Uri.EscapeDataString("我")}");
        }

        [Fact]
        public void ReplaceTest()
        {
            var uriString = new UriString("http://www.webapiclient.com");
            uriString.Replace("a", "a", out var replaced);
            Assert.False(replaced);

            uriString = new UriString("http://www.webapiclient.com/path/?x={x}&y={Y}");
            uriString = uriString.Replace("x", "x", out replaced);
            Assert.True(replaced);
            Assert.Equal("http://www.webapiclient.com/path/?x=x&y={Y}", uriString.ToString());

            uriString = new UriString("http://www.webapiclient.com/path/?x={x}&y={Y}");
            uriString = uriString.Replace("y", "y", out replaced);
            Assert.True(replaced);
            Assert.Equal("http://www.webapiclient.com/path/?x={x}&y=y", uriString.ToString());
        }

        [Fact]
        public void ToUriTest()
        {
            var uriString = new UriString("http://www.webapiclient.com");
            var uri = uriString.ToUri();
            Assert.Equal(new Uri("http://www.webapiclient.com"), uri);

            uriString = new UriString(new Uri("http://www.webapiclient.com"));
            uri = uriString.ToUri();
            Assert.Equal(new Uri("http://www.webapiclient.com"), uri);
        }
    }
}
