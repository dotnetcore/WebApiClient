using System;
using System.Collections.Generic;
using System.Text;
using WebApiClient;
using Xunit;
using System.Linq;

namespace WebApiClient.Test.Internal
{
    public class UriEditorTest
    {
        [Fact]
        public void BuildTest()
        {
            var encoding = Encoding.UTF8;

            var url = new Uri("http://www.webapiclient.com");
            var editor = new UriEditor(url, encoding);
            Assert.False(editor.Replace("a", "a"));
            editor.AddQuery("a", "a");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/?a=a");

            url = new Uri("http://www.webapiclient.com/path");
            editor = new UriEditor(url, encoding);
            editor.AddQuery("a", "a");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path?a=a");

            url = new Uri("http://www.webapiclient.com/path/");
            editor = new UriEditor(url, encoding);
            editor.AddQuery("a", "a");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path/?a=a");


            url = new Uri("http://www.webapiclient.com/path/?");
            editor = new UriEditor(url, encoding);
            editor.AddQuery("a", "a");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path/?a=a");

            url = new Uri("http://www.webapiclient.com/path?x=1");
            editor = new UriEditor(url, encoding);
            editor.AddQuery("a", "a");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path?x=1&a=a");


            url = new Uri("http://www.webapiclient.com/path?x=1&");
            editor = new UriEditor(url, encoding);
            editor.AddQuery("a", "a");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path?x=1&a=a");


            url = new Uri("http://www.webapiclient.com/path?x=1&");
            editor = new UriEditor(url, encoding);
            editor.AddQuery("a", "我");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path?x=1&a=我");


            url = new Uri("http://www.webapiclient.com/path/?x=1&");
            editor = new UriEditor(url, encoding);
            editor.AddQuery("a", "我");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path/?x=1&a=我");


            url = new Uri("http://www.webapiclient.com/path/?x={x}&");
            editor = new UriEditor(url, encoding);
            editor.Replace("x", "你");
            editor.AddQuery("a", "我");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path/?x=你&a=我");

            url = new Uri("http://www.webapiclient.com");
            editor = new UriEditor(url, encoding);
            editor.AddQuery("a", "我");
            editor.AddQuery("b", "你");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/?a=我&b=你");


            url = new Uri("http://u:p@www.webapiclient.com/x/y/z?a=1&b2=2#tag");
            editor = new UriEditor(url);
            editor.SetPath("/");
            Assert.True(editor.Uri.ToString() == "http://u:p@www.webapiclient.com/?a=1&b2=2#tag");
        }
    }
}
