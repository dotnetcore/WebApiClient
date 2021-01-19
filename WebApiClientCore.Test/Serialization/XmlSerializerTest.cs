using System.Text;
using WebApiClientCore.Serialization;
using Xunit;

namespace WebApiClientCore.Test.Serialization
{
    public class XmlSerializerTest
    {
        [Fact]
        public void ReadWriteTest()
        {
            var obj1 = new FormatModel { Age = 18, Name = "老九" };
            var xml = XmlSerializer.Serialize(obj1, null);
            var obj2 = XmlSerializer.Deserialize(xml, typeof(FormatModel), null);
            Assert.True(obj1.Equals(obj2));
        }

        [Fact]
        public void EncodingTest()
        {
            var obj1 = new FormatModel { Age = 18, Name = "老九" };
           
            var opt = new System.Xml.XmlWriterSettings
            {
                Encoding = Encoding.Unicode
            };

            var xml = XmlSerializer.Serialize(obj1, opt);
            Assert.Contains(opt.Encoding.WebName, xml);
        }
    }
}
