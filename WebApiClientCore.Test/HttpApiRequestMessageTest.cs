using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebApiClientCore.Exceptions;
using Xunit;

namespace WebApiClientCore.Test
{
    public class HttpApiRequestMessageTest
    {
        [Fact]
        public void AddUrlQueryTest()
        {
            var request = new HttpApiRequestMessage();
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
            var reqeust = new HttpApiRequestMessage();
            await Assert.ThrowsAsync<NotSupportedException>(() => reqeust.AddFormFieldAsync("name", "value"));

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

{HttpUtility.UrlEncode(value, Encoding.UTF8)}";
            }

            var reqeust = new HttpApiRequestMessage();
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
