using System;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;
using WebApiClientCore.Implementations;
using WebApiClientCore.Internals;
using Xunit;

namespace WebApiClientCore.Test.Implementations
{
    public class HttpApiRequestMessageTest
    {
        [Fact]
        public void MakeRequestUriTest()
        {
            var request = new HttpApiRequestMessageImpl();
            var uri = request.MakeRequestUri(new Uri("a", UriKind.RelativeOrAbsolute));
            Assert.Equal("a", uri.OriginalString);

            request = new HttpApiRequestMessageImpl();
            uri = request.MakeRequestUri(new Uri("http://a.com", UriKind.RelativeOrAbsolute));
            Assert.Equal("http://a.com", uri.OriginalString);

            request = new HttpApiRequestMessageImpl();
            uri = request.MakeRequestUri(new Uri("http://a.com/a", UriKind.RelativeOrAbsolute));
            Assert.Equal("http://a.com/a", uri.OriginalString);
        }

        [Fact]
        public void MakeRequestUri2Test()
        {
            var request = new HttpApiRequestMessageImpl();
            request.RequestUri = new Uri("http://b.com/", UriKind.RelativeOrAbsolute);
            var uri = request.MakeRequestUri(new Uri("a", UriKind.RelativeOrAbsolute));
            Assert.Equal("http://b.com/a", uri.OriginalString);

            request = new HttpApiRequestMessageImpl();
            request.RequestUri = new Uri("http://b.com/", UriKind.RelativeOrAbsolute);
            uri = request.MakeRequestUri(new Uri("http://a.com", UriKind.RelativeOrAbsolute));
            Assert.Equal("http://a.com", uri.OriginalString);

            request = new HttpApiRequestMessageImpl();
            request.RequestUri = new Uri("http://b.com/", UriKind.RelativeOrAbsolute);
            uri = request.MakeRequestUri(new Uri("http://a.com/a", UriKind.RelativeOrAbsolute));
            Assert.Equal("http://a.com/a", uri.OriginalString);
        }

        [Fact]
        public void MakeRequestUri3Test()
        {
            var request = new HttpApiRequestMessageImpl();
            request.RequestUri = new Uri("http://b.com/x", UriKind.RelativeOrAbsolute);
            var uri = request.MakeRequestUri(new Uri("a", UriKind.RelativeOrAbsolute));
            Assert.Equal("http://b.com/a", uri.OriginalString);

            request = new HttpApiRequestMessageImpl();
            request.RequestUri = new Uri("http://b.com/x", UriKind.RelativeOrAbsolute);
            uri = request.MakeRequestUri(new Uri("http://a.com", UriKind.RelativeOrAbsolute));
            Assert.Equal("http://a.com/x", uri.OriginalString);

            request = new HttpApiRequestMessageImpl();
            request.RequestUri = new Uri("http://b.com/x", UriKind.RelativeOrAbsolute);
            uri = request.MakeRequestUri(new Uri("http://a.com/", UriKind.RelativeOrAbsolute));
            Assert.Equal("http://a.com/x", uri.OriginalString);

            request = new HttpApiRequestMessageImpl();
            request.RequestUri = new Uri("http://b.com/x", UriKind.RelativeOrAbsolute);
            uri = request.MakeRequestUri(new Uri("http://a.com/a", UriKind.RelativeOrAbsolute));
            Assert.Equal("http://a.com/a", uri.OriginalString);
        }

        [Fact]
        public void AddUrlQueryTest()
        {
            var request = new HttpApiRequestMessageImpl();
            Assert.Throws<ApiInvalidConfigException>(() => request.AddUrlQuery("xKey", "xValue"));

            request.RequestUri = new Uri("http://webapiclient.com/");
            request.AddUrlQuery("xKey", "xValue");
            Assert.True(request.RequestUri == new Uri("http://webapiclient.com?xKey=xValue"));

            Assert.Throws<ArgumentNullException>(() => request.AddUrlQuery(null, string.Empty));

            request.AddUrlQuery("yKey", "yValue");
            Assert.True(request.RequestUri == new Uri("http://webapiclient.com?xKey=xValue&yKey=yValue"));

            var unicodeValue = Uri.EscapeDataString("老九");
            request.AddUrlQuery("zKey", "老九");
            var url = new Uri($"http://webapiclient.com?xKey=xValue&yKey=yValue&zKey={unicodeValue}");
            Assert.True(request.RequestUri == url);
        }

        [Fact]
        public async Task AddFormFiledAsyncTest()
        {
            var reqeust = new HttpApiRequestMessageImpl();

            // 已经移除Get或Head限制
            // await Assert.ThrowsAsync<NotSupportedException>(() => reqeust.AddFormFieldAsync("name", "value"));

            reqeust.Method = System.Net.Http.HttpMethod.Post;
            reqeust.RequestUri = new Uri("http://webapiclient.com");
            await reqeust.AddFormFieldAsync("name", "laojiu");
            await reqeust.AddFormFieldAsync(new[] { new KeyValue("age", "18") });

            var body = await reqeust.Content.ReadAsStringAsync();
            Assert.Equal("name=laojiu&age=18", body);
            Assert.True(reqeust.Content.Headers.ContentType.MediaType == "application/x-www-form-urlencoded");
        }


        [Fact]
        public async Task AddFormDataTextTest()
        {
            string get(string name, string value)
            {
                return $@"Content-Disposition: form-data; name=""{name}""

{HttpUtil.UrlEncode(value, Encoding.UTF8)}";
            }

            var reqeust = new HttpApiRequestMessageImpl();
            reqeust.Method = System.Net.Http.HttpMethod.Post;
            reqeust.RequestUri = new Uri("http://webapiclient.com");
            reqeust.AddFormDataText("name", "laojiu");
            reqeust.AddFormDataText(new[] { new KeyValue("age", "18") });

            await Assert.ThrowsAsync<NotSupportedException>(() => reqeust.AddFormFieldAsync("key", "value"));

            var body = await reqeust.Content.ReadAsStringAsync();
            Assert.Contains(get("name", "laojiu"), body);
            Assert.Contains(get("age", "18"), body);
            Assert.True(reqeust.Content.Headers.ContentType.MediaType == "multipart/form-data");
        }
    }
}
