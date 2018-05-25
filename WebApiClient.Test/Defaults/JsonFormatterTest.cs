using Xunit;

namespace WebApiClient.Test.Defaults
{
    public class JsonFormatterTest
    {
        [Fact]
        public void Test_All_Methods()
        {
            var obj1 = new FormatModel { Age = 18, Name = "老九" };
            var formatter = new WebApiClient.Defaults.JsonFormatter();
            var json = formatter.Serialize(obj1, null);
            var obj2 = formatter.Deserialize(json, typeof(FormatModel));
            Assert.True(obj1.Equals(obj2));


            var dic = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();
            dic.TryAdd("Key", "Value");

            var formatter2 = new WebApiClient.Defaults.JsonFormatter();
            var json2 = formatter2.Serialize(dic, new FormatOptions { UseCamelCase = true });
            Assert.Contains("key", json2);
        }
    }
}
