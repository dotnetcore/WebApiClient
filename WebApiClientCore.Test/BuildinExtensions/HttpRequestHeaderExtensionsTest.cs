using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Xunit;

namespace WebApiClientCore.Test.BuildinExtensions
{
    public class HttpRequestHeaderExtensionsTest
    {
        [Fact]
        public void ToHeaderNameTest()
        {
            Assert.Equal("Accept", HttpRequestHeader.Accept.ToHeaderName());
            Assert.Equal("Accept-Charset", HttpRequestHeader.AcceptCharset.ToHeaderName());

            foreach (var item in Enum.GetValues<HttpRequestHeader>())
            {
                var name = Enum.GetName(item);
                var field = typeof(HttpRequestHeader).GetField(name!);
                var headerName = field?.GetCustomAttribute<DisplayAttribute>()?.Name;
                Assert.Equal(headerName, item.ToHeaderName());
            }
        }
    }
}