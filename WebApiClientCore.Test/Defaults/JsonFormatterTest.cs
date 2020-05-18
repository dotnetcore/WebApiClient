using System.Text;
using Xunit;

namespace WebApiClientCore.Test.Defaults
{
    public class JsonFormatterTest
    {
        [Fact]
        public void ReadWriteTest()
        {
            var options = HttpApiOptions.CreateDefaultJsonOptions();

            var obj1 = new FormatModel { Age = 18, Name = "老九" };
            var formatter = new WebApiClientCore.Defaults.JsonFormatter();
            var json = formatter.Serialize(obj1, options);
            var obj2 = formatter.Deserialize(json, typeof(FormatModel), options);
            Assert.True(obj1.Equals(obj2));

            var dic = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();
            dic.TryAdd("Key", "Value");

            var json2 = Encoding.UTF8.GetString(formatter.Serialize(dic, options));
            Assert.Contains("key", json2);
        }
    }
}
