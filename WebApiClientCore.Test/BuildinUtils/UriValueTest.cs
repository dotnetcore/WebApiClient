using System;
using Xunit;

namespace WebApiClientCore.Test.BuildinUtils
{
    public class UriValueTest
    {
        [Fact]
        public void AddQueryTest()
        {
            var uriValue = new UriValue("http://www.webapiclient.com");
            uriValue = uriValue.AddQuery("a", "a");
            Assert.Equal("http://www.webapiclient.com/?a=a", uriValue.ToString());

            uriValue = new UriValue("http://www.webapiclient.com/path");
            uriValue = uriValue.AddQuery("a", "a");
            Assert.Equal("http://www.webapiclient.com/path?a=a", uriValue.ToString());

            uriValue = new UriValue("http://www.webapiclient.com/path/");
            uriValue = uriValue.AddQuery("a", "a");
            Assert.Equal("http://www.webapiclient.com/path/?a=a", uriValue.ToString());


            uriValue = new UriValue("http://www.webapiclient.com/path/?");
            uriValue = uriValue.AddQuery("a", "a");
            Assert.Equal("http://www.webapiclient.com/path/?a=a", uriValue.ToString());

            uriValue = new UriValue("http://www.webapiclient.com/path?x=1");
            uriValue = uriValue.AddQuery("a", "a");
            Assert.Equal("http://www.webapiclient.com/path?x=1&a=a", uriValue.ToString());


            uriValue = new UriValue("http://www.webapiclient.com/path?x=1&");
            uriValue = uriValue.AddQuery("a", "a");
            Assert.Equal("http://www.webapiclient.com/path?x=1&a=a", uriValue.ToString());

            uriValue = new UriValue("http://www.webapiclient.com/path?x=1&");
            uriValue = uriValue.AddQuery("a", "我");
            Assert.Equal($"http://www.webapiclient.com/path?x=1&a={Uri.EscapeDataString("我")}", uriValue.ToString());


            uriValue = new UriValue("http://www.webapiclient.com/path/?x=1&");
            uriValue = uriValue.AddQuery("a", "我");
            Assert.True(uriValue.ToString() == $"http://www.webapiclient.com/path/?x=1&a={Uri.EscapeDataString("我")}");
        }

        [Fact]
        public void ReplaceTest()
        {
            var uriValue = new UriValue("http://www.webapiclient.com");
            uriValue.Replace("a", "a", out var replaced);
            Assert.False(replaced);

            uriValue = new UriValue("http://www.webapiclient.com/path/?x={x}&y={Y}");
            uriValue = uriValue.Replace("x", "x", out replaced);
            Assert.True(replaced);
            Assert.Equal("http://www.webapiclient.com/path/?x=x&y={Y}", uriValue.ToString());

            uriValue = new UriValue("http://www.webapiclient.com/path/?x={x}&y={Y}");
            uriValue = uriValue.Replace("y", "y", out replaced);
            Assert.True(replaced);
            Assert.Equal("http://www.webapiclient.com/path/?x={x}&y=y", uriValue.ToString());
        }

        [Fact]
        public void ToUriTest()
        {
            var uriValue = new UriValue("http://www.webapiclient.com");
            var uri = uriValue.ToUri();
            Assert.Equal(new Uri("http://www.webapiclient.com"), uri);

            uriValue = new UriValue(new Uri("http://www.webapiclient.com"));
            uri = uriValue.ToUri();
            Assert.Equal(new Uri("http://www.webapiclient.com"), uri);
        }
    }
}
