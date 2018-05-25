using System.Linq;
using Xunit;


namespace WebApiClient.Test.Defaults
{
    public class KeyValueFormatterTest
    {
        [Fact]
        public void SerializeTest()
        {
            var obj1 = new FormatModel { Age = 18, Name = "老九" };
            var formatter = new WebApiClient.Defaults.KeyValueFormatter();
            var kvs = formatter.Serialize("pName", obj1, null)
                .ToDictionary(item => item.Key, item => item.Value);

            Assert.True(kvs.Count == 2);
            Assert.True(kvs["Name"] == "老九");
            Assert.True(kvs["Age"] == "18");


            kvs = formatter.Serialize("pName", 30, null)
               .ToDictionary(item => item.Key, item => item.Value);

            Assert.True(kvs.Count == 1);
            Assert.True(kvs["pName"] == "30");



            var dic = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();
            dic.TryAdd("Key", "Value");

            var formatter2 = new WebApiClient.Defaults.KeyValueFormatter();
            var kvs2 = formatter2.Serialize( null,dic, new FormatOptions { UseCamelCase = true });
            var kvs3 = formatter2.Serialize(null, dic.ToArray(), new FormatOptions { UseCamelCase = true });
            Assert.True(kvs2.First().Key == "key");
            Assert.True(kvs3.First().Key == "key");
        }
    }
}
