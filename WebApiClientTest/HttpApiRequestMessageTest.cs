using System;
using Xunit;
using WebApiClient;
using WebApiClient.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClientTest
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
            await reqeust.AddFormFieldAsync("name", "value");

            var body = await reqeust.Content.ReadAsStringAsync();
            Assert.True(body == "name=value");
            Assert.True(reqeust.Content.Headers.ContentType.MediaType == "application/x-www-form-urlencoded");
           
        }
    }
}
