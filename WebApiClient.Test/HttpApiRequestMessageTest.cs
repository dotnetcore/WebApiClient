using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using Xunit;

namespace WebApiClient.Test
{
    public class HttpApiRequestMessageTest
    {
        [Fact]
        public void TimeoutTest()
        {
            var request = new HttpApiRequestMessage() { Timeout = TimeSpan.FromDays(1d) };
            Assert.True(request.Timeout == TimeSpan.FromDays(1d));
        }

        [Fact]
        public void AddUrlQueryTest()
        {
            var request = new HttpApiRequestMessage();
            Assert.Throws<HttpApiConfigException>(() => request.AddUrlQuery("xKey", "xValue"));

            request.RequestUri = new Uri("http://webapiclient.com/");
            request.AddUrlQuery("xKey", "xValue");
            Assert.True(request.RequestUri == new Uri("http://webapiclient.com?xKey=xValue"));

            Assert.Throws<ArgumentNullException>(() => request.AddUrlQuery(null, string.Empty));

            var kv = new KeyValuePair<string, string>("yKey", "yValue");
            request.AddUrlQuery(kv);
            Assert.True(request.RequestUri == new Uri("http://webapiclient.com?xKey=xValue&yKey=yValue"));

            var unicodeValue = HttpUtility.UrlEncode("老九", Encoding.UTF8);
            request.AddUrlQuery(new[] { new KeyValuePair<string, string>("zKey", "老九") }, Encoding.UTF8);
            var url = new Uri($"http://webapiclient.com?xKey=xValue&yKey=yValue&zKey={unicodeValue}");
            Assert.True(request.RequestUri == url);
        }

        [Fact]
        public async Task AddFormFiledAsyncTest()
        {
            var reqeust = new HttpApiRequestMessage();
            await Assert.ThrowsAsync<NotSupportedException>(() => reqeust.AddFormFieldAsync("name", "value"));

            reqeust.Method = System.Net.Http.HttpMethod.Post;
            reqeust.RequestUri = new Uri("http://webapiclient.com");
            await reqeust.AddFormFieldAsync("name", "laojiu");
            await reqeust.AddFormFieldAsync(new[] { new KeyValuePair<string, string>("age", "18") });

            var body = await reqeust.Content.ReadAsStringAsync();
            Assert.Equal("name=laojiu&age=18", body);
            Assert.True(reqeust.Content.Headers.ContentType.MediaType == "application/x-www-form-urlencoded");
        }


        [Fact]
        public async Task AddMulitpartTextTest()
        {
            string get(string name, string value)
            {
                return $@"Content-Disposition: form-data; name=""{name}""

{HttpUtility.UrlEncode(value, Encoding.UTF8)}";
            }

            var reqeust = new HttpApiRequestMessage();
            reqeust.Method = System.Net.Http.HttpMethod.Post;
            reqeust.RequestUri = new Uri("http://webapiclient.com");
            reqeust.AddMulitpartText("name", "laojiu");
            reqeust.AddMulitpartText(new[] { new KeyValuePair<string, string>("age", "18") });

            await Assert.ThrowsAsync<NotSupportedException>(() => reqeust.AddFormFieldAsync("key", "value"));

            var body = await reqeust.Content.ReadAsStringAsync();
            Assert.Contains(get("name", "laojiu"), body);
            Assert.Contains(get("age", "18"), body);
            Assert.True(reqeust.Content.Headers.ContentType.MediaType == "multipart/form-data");
        }

        [Fact ]
        public void SetCookieTest()
        {
            var cookie = "a=1; b=2";
            var reqeust = new HttpApiRequestMessage();
            reqeust.SetCookie(cookie);
            reqeust.Headers.TryGetValues("Cookie", out IEnumerable<string> values);
            var cookies = values.ToArray();
            Assert.True(cookies.Length == 1 && cookies.First() == "a=1; b=2");
        }
    }
}
