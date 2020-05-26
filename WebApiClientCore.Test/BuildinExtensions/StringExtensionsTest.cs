using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace WebApiClientCore.Test.BuildinExtensions
{
    public class StringExtensionsTest
    {
        [Fact]
        public void RepaceIgnoreCaseTest()
        {
            var str = "WebApiClientCore.Benchmarks.StringReplaces.WebApiClientCore";
            Assert.True(str.RepaceIgnoreCase("core", "CORE", out var newStr));
            Assert.Equal("WebApiClientCORE.Benchmarks.StringReplaces.WebApiClientCORE", newStr);

            str = "AbccBAd";
            Assert.True(str.RepaceIgnoreCase("A", "x", out var newStr2));
            Assert.Equal("xbccBxd", newStr2);

            str = "abc";
            Assert.False(str.RepaceIgnoreCase("x", "x", out var newStr3));
            Assert.Equal(str, newStr3);

            str = "aaa";
            Assert.True(str.RepaceIgnoreCase("A", "x", out var newStr4));
            Assert.Equal("xxx", newStr4);

            Assert.True(str.RepaceIgnoreCase("a", null, out var newStr5));
            Assert.Equal("", newStr5);

            Assert.Throws<ArgumentNullException>(() => "22".RepaceIgnoreCase(null, null, out _));
        }
    }
}
