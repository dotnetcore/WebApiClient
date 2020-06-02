using System;
using System.Text;
using Xunit;

namespace WebApiClientCore.Test.ResponseCaches
{
    public class UriEditorTest
    {
        [Fact]
        public void BuildTest()
        {
            var url = new Uri("http://www.webapiclient.com");
            var editor = new UriEditor(url);
            Assert.False(editor.Replace("a", "a"));
            editor.AddQuery("a", "a");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/?a=a");

            url = new Uri("http://www.webapiclient.com/path");
            editor = new UriEditor(url);
            editor.AddQuery("a", "a");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path?a=a");

            url = new Uri("http://www.webapiclient.com/path/");
            editor = new UriEditor(url);
            editor.AddQuery("a", "a");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path/?a=a");


            url = new Uri("http://www.webapiclient.com/path/?");
            editor = new UriEditor(url);
            editor.AddQuery("a", "a");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path/?a=a");

            url = new Uri("http://www.webapiclient.com/path?x=1");
            editor = new UriEditor(url);
            editor.AddQuery("a", "a");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path?x=1&a=a");


            url = new Uri("http://www.webapiclient.com/path?x=1&");
            editor = new UriEditor(url);
            editor.AddQuery("a", "a");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path?x=1&a=a");


            url = new Uri("http://www.webapiclient.com/path?x=1&");
            editor = new UriEditor(url);
            editor.AddQuery("a", "我");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path?x=1&a=我");


            url = new Uri("http://www.webapiclient.com/path/?x=1&");
            editor = new UriEditor(url);
            editor.AddQuery("a", "我");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path/?x=1&a=我");


            url = new Uri("http://www.webapiclient.com/path/?x={x}&y={Y}&");
            editor = new UriEditor(url);

            editor.AddQuery("a", "我");
            Assert.True(editor.Replace("x", "你"));
            Assert.False(editor.Replace("x", "你"));

            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/path/?x=你&y={Y}&a=我");

            url = new Uri("http://www.webapiclient.com");
            editor = new UriEditor(url);
            editor.AddQuery("a", "我");
            editor.AddQuery("b", "你");
            Assert.True(editor.Uri.ToString() == "http://www.webapiclient.com/?a=我&b=你");

        }
    }
}
