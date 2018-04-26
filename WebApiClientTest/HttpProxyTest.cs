using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using WebApiClient;
using Xunit;

namespace WebApiClientTest
{
    public class HttpProxyTest
    {
        [Fact]
        public void CtorTest()
        {
            var target = new Uri("https://www.baidu.com/");
            var uri = "http://localhost:5533/";
            var proxy = new HttpProxy(uri);

            Assert.True(proxy.Host == "localhost" && proxy.Port == 5533);
            Assert.True(proxy.GetProxy(target) == new Uri(uri));
            Assert.True(proxy.UserName == null);
            Assert.True(((IWebProxy)proxy).Credentials == null);


            proxy = new HttpProxy(new Uri(uri));
            Assert.True(proxy.Host == "localhost" && proxy.Port == 5533);
            Assert.True(proxy.GetProxy(target) == new Uri(uri));
            Assert.True(proxy.UserName == null);
            Assert.True(((IWebProxy)proxy).Credentials == null);


            proxy = new HttpProxy("localhost", 5533, "laojiu", "123456");
            Assert.True(proxy.Host == "localhost" && proxy.Port == 5533);
            Assert.True(proxy.GetProxy(target) == new Uri(uri));
            Assert.True(proxy.UserName == "laojiu");
            Assert.True(proxy.Password == "123456");
            Assert.True(((IWebProxy)proxy).Credentials.GetCredential(null, null).UserName == "laojiu");
            Assert.True(((IWebProxy)proxy).Credentials.GetCredential(null, null).Password == "123456");
        }

        [Fact]
        public void FromWebProxyTest()
        {
            var target = new Uri("https://www.baidu.com/");
            var p = new WebProxy("127.0.0.1", 5000) { Credentials = new NetworkCredential("abc", "123") };
            var proxy = HttpProxy.FromWebProxy(p, target);
            Assert.True(proxy.Host == "127.0.0.1" && proxy.Port == 5000);
            Assert.True(proxy.GetProxy(target) == new Uri("http://127.0.0.1:5000"));
            Assert.True(proxy.UserName == "abc");
            Assert.True(proxy.Password == "123");
            Assert.True(((IWebProxy)proxy).Credentials.GetCredential(null, null).UserName == "abc");
            Assert.True(((IWebProxy)proxy).Credentials.GetCredential(null, null).Password == "123");

        }

        [Fact]
        public void RangeTest()
        {
            var proxys = HttpProxy.Range(IPAddress.Parse("221.122.0.1"), 8080, 5).ToArray();
            Assert.True(proxys.Length == 5);
            Assert.True(proxys.First().Host == "221.122.0.1");
            Assert.True(proxys.Last().Host == "221.122.0.5");
        }

        [Fact]
        public void IsProxyEqualsTest()
        {
            var x = new HttpProxy("localhost", 80, "abc", "123");
            var y = new HttpProxy("localhost", 80, "abc", "456");
            var z = new WebProxy("http://localhost") { Credentials = new NetworkCredential("abc", "456") };

            Assert.False(HttpProxy.IsProxyEquals(x, y));
            Assert.True(HttpProxy.IsProxyEquals(z, y));
        }
    }
}
