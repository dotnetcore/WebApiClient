using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.HttpContents;
using Xunit;

namespace WebApiClientCore.Test.HttpContents
{
    public class JsonContentTest
    {
        [Fact]
        public async Task Utf8JsonTest()
        {
            var options = new WebApiClientCore.HttpApiOptions();
            var content = new JsonContent("at我", options.JsonSerializeOptions);
            Assert.Equal(content.GetEncoding(), Encoding.UTF8);
            var text = await content.ReadAsStringAsync();
            Assert.Equal("\"at我\"", text);
        }

        [Fact]
        public async Task Utf16JsonTest()
        {
            var options = new WebApiClientCore.HttpApiOptions();
            var content = new JsonContent("at我", options.JsonSerializeOptions, Encoding.Unicode);
            Assert.Equal(content.GetEncoding(), Encoding.Unicode);
            var text = await content.ReadAsStringAsync();
            Assert.Equal("\"at我\"", text);
        }
    }
}
