using System;
using Xunit;
using WebApiClient;
using WebApiClient.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using System.Linq;


namespace WebApiClientTest.Defaults
{
    public class KeyValueFormatterTest
    {
        [Fact]
        public void Test_All_Methods()
        {
            var obj1 = new FormatModel { Age = 18, Name = "老九" };
            var formatter = new WebApiClient.Defaults.KeyValueFormatter();
            var kvs = formatter.Serialize("pName", obj1, null).ToDictionary(item => item.Key, item => item.Value);

            Assert.True(kvs.Count == 2);
            Assert.True(kvs["Name"] == "老九");
            Assert.True(kvs["Age"] == "18");
        }
    }
}
