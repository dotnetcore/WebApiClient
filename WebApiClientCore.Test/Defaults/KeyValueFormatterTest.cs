using System.Linq;
using WebApiClientCore.Defaults;
using Xunit;


namespace WebApiClientCore.Test.Defaults
{
    public class KeyValueFormatterTest
    {
        [Fact]
        public void SerializeTest()
        {           
            var obj1 = new FormatModel { Age = 18, Name = "laojiu" };
            var formatter = new KeyValueFormatter();
            var kvs = formatter.Serialize("pName", obj1, null)
                .ToDictionary(item => item.Key, item => item.Value);

            Assert.True(kvs.Count == 2);
            Assert.True(kvs["Name"] == "laojiu");
            Assert.True(kvs["Age"] == "18");


            kvs = formatter.Serialize("pName", 30, null)
               .ToDictionary(item => item.Key, item => item.Value);

            Assert.True(kvs.Count == 1);
            Assert.True(kvs["pName"] == "30");



            var dic = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();
            dic.TryAdd("Key", "Value");

            var options = HttpApiOptions.CreateDefaultJsonOptions();
            var kvs2 = formatter.Serialize("dic", dic, options ); 
            Assert.True(kvs2.First().Key == "key"); 


            Assert.True(formatter.Serialize("null", null, null).Any());
        }
    }
}
