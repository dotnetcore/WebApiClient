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

        }
    }
}
