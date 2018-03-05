using System;
using Xunit;
using WebApiClient;
using WebApiClient.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;

namespace WebApiClientTest.Defaults
{
    public class FormatModel
    {
        public int Age { get; set; }

        public string Name { get; set; }

        public override int GetHashCode()
        {
            return Age.GetHashCode() ^ (Name == null ? 0 : Name.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is FormatModel o)
            {
                return o.Age == this.Age && o.Name == this.Name;
            }
            return false;
        }
    }

    public class XmlFormaterTest
    { 
        [Fact]
        public void Test_All_Methods()
        {
            var obj1 = new FormatModel { Age = 18, Name = "老九" };
            var formatter = new WebApiClient.Defaults.XmlFormatter();
            var xml = formatter.Serialize(obj1, Encoding.UTF8);     
            var obj2 = formatter.Deserialize(xml, typeof(FormatModel));
            Assert.True(obj1.Equals(obj2));
        }
    }
}
