using System;
using System.Collections.Generic;
using System.Text;
using WebApiClient;
using Xunit;
using System.Linq;

namespace WebApiClient.Test.Internal
{
    public class UrlBuilderTest
    {
        [Fact]
        public void BuildTest()
        {
            var encoding = Encoding.UTF8;

            var url = new Uri("http://www.webapiclient.com");
            var builder = new UrlBuilder(url, encoding);
            Assert.False(builder.Replace("a", "a"));
            builder.AddQuery("a", "a");
            Assert.True(builder.Uri.ToString() == "http://www.webapiclient.com/?a=a");

            url = new Uri("http://www.webapiclient.com/path");
            builder = new UrlBuilder(url, encoding);
            builder.AddQuery("a", "a");
            Assert.True(builder.Uri.ToString() == "http://www.webapiclient.com/path?a=a");

            url = new Uri("http://www.webapiclient.com/path/");
            builder = new UrlBuilder(url, encoding);
            builder.AddQuery("a", "a");
            Assert.True(builder.Uri.ToString() == "http://www.webapiclient.com/path/?a=a");


            url = new Uri("http://www.webapiclient.com/path/?");
            builder = new UrlBuilder(url, encoding);
            builder.AddQuery("a", "a");
            Assert.True(builder.Uri.ToString() == "http://www.webapiclient.com/path/?a=a");

            url = new Uri("http://www.webapiclient.com/path?x=1");
            builder = new UrlBuilder(url, encoding);
            builder.AddQuery("a", "a");
            Assert.True(builder.Uri.ToString() == "http://www.webapiclient.com/path?x=1&a=a");


            url = new Uri("http://www.webapiclient.com/path?x=1&");
            builder = new UrlBuilder(url, encoding);
            builder.AddQuery("a", "a");
            Assert.True(builder.Uri.ToString() == "http://www.webapiclient.com/path?x=1&a=a");


            url = new Uri("http://www.webapiclient.com/path?x=1&");
            builder = new UrlBuilder(url, encoding);
            builder.AddQuery("a", "我");
            Assert.True(builder.Uri.ToString() == "http://www.webapiclient.com/path?x=1&a=我");


            url = new Uri("http://www.webapiclient.com/path/?x=1&");
            builder = new UrlBuilder(url, encoding);
            builder.AddQuery("a", "我");
            Assert.True(builder.Uri.ToString() == "http://www.webapiclient.com/path/?x=1&a=我");


            url = new Uri("http://www.webapiclient.com/path/?x={x}&");
            builder = new UrlBuilder(url, encoding);
            builder.Replace("x", "你");
            builder.AddQuery("a", "我");
            Assert.True(builder.Uri.ToString() == "http://www.webapiclient.com/path/?x=你&a=我");

            url = new Uri("http://www.webapiclient.com");
            builder = new UrlBuilder(url, encoding);
            builder.AddQuery("a", "我");
            builder.AddQuery("b", "你");
            Assert.True(builder.Uri.ToString() == "http://www.webapiclient.com/?a=我&b=你");


            url = new Uri("http://u:p@www.webapiclient.com/x/y/z?a=1&b2=2#tag");
            builder = new UrlBuilder(url);
            builder.SetPath("/");
            Assert.True(builder.Uri.ToString() == "http://u:p@www.webapiclient.com/?a=1&b2=2#tag");
        }
    }
}
