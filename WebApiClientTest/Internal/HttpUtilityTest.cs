using System;
using System.Collections.Generic;
using System.Text;
using WebApiClient;
using Xunit;
using System.Linq;

namespace WebApiClientTest.Internal
{
    public class HttpUtilityTest
    {
        [Fact]
        public void ParseCookieTest()
        {
            var cookie = "a=1; b=我";
            var cs = HttpUtility.ParseCookie(cookie, true).ToArray();
            Assert.True(cs.Length == 2);

            var a = cs.FirstOrDefault(item => item.Name == "a");
            Assert.True(a != null && a.Value == "1");

            var b = cs.FirstOrDefault(item => item.Name == "b");
            Assert.True(b != null && b.Value == HttpUtility.UrlEncode("我", Encoding.UTF8));
        }
    }
}
