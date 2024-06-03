using System;
using Xunit;

namespace WebApiClientCore.Test
{
    public class HttpPathTest
    {
        [Fact]
        public void NullHttpPathTest()
        {
            var path = HttpPath.Create(null);
            Uri? input = null;
            var output = path.MakeUri(input);
            Assert.Null(output);

            input = new Uri("http://a.com");
            output = path.MakeUri(input);
            Assert.Equal(input, output);

            input = new Uri("/aa/bb", UriKind.Relative);
            output = path.MakeUri(input);
            Assert.Equal(input, output);
        }

        [Fact]
        public void RelativePathTest()
        {
            var value = new Uri("/x", UriKind.Relative);

            var path = HttpPath.Create(value.OriginalString);
            var output = path.MakeUri(null);
            Assert.Equal(value, output);

            output = path.MakeUri(new Uri("/a/b", UriKind.Relative));
            Assert.Equal(value, output);

            output = path.MakeUri(new Uri("http://a.com/a/b"));
            Assert.Equal(new Uri("http://a.com/x"), output);
        }

        [Fact]
        public void AbsolutePathTest()
        {
            var value = new Uri("http://a.com/x");

            var path = HttpPath.Create(value.OriginalString);
            var output = path.MakeUri(null);
            Assert.Equal(value, output);

            output = path.MakeUri(new Uri("/a/b", UriKind.Relative));
            Assert.Equal(value, output);

            output = path.MakeUri(new Uri("http://a.com/a/b"));
            Assert.Equal(value, output);
        }
    }
}
