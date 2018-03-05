using System.Linq;
using WebApiClient.Defaults.KeyValueFormats;
using WebApiClient.Defaults.KeyValueFormats.Converters;
using Xunit;

namespace WebApiClientTest.Defaults.KeyValueFormats
{
    public class NullValueConverterTest
    {
        [Fact]
        public void InvokeTest()
        {
            var first = ConverterMiddleware.Link(new NullValueConverter());
            var context = new ConvertContext("name", null, 0, null);
            var kvs = first.Invoke(context)
                .ToDictionary(item => item.Key, item => item.Value);

            Assert.True(kvs["name"] == null);
        }
    }
}
