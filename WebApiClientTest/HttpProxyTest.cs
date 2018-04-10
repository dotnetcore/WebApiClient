using System;
using System.Collections.Generic;
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


            var p = new WebProxy("127.0.0.1", 5000) { Credentials = new NetworkCredential("abc", "123") };
            proxy = HttpProxy.FromWebProxy(p, target);
            Assert.True(proxy.Host == "127.0.0.1" && proxy.Port == 5000);
            Assert.True(proxy.GetProxy(target) == new Uri("http://127.0.0.1:5000"));
            Assert.True(proxy.UserName == "abc");
            Assert.True(proxy.Password == "123");
            Assert.True(((IWebProxy)proxy).Credentials.GetCredential(null, null).UserName == "abc");
            Assert.True(((IWebProxy)proxy).Credentials.GetCredential(null, null).Password == "123");

        }
    }
}
