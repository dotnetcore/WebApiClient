using System.Text;
using Xunit;

namespace WebApiClientCore.Test.Defaults
{
    public class XmlFormaterTest
    {
        [Fact]
        public void ReadWriteTest()
        {
            var obj1 = new FormatModel { Age = 18, Name = "老九" };
            var formatter = new WebApiClientCore.Defaults.XmlFormatter();
            var xml = formatter.Serialize(obj1, Encoding.UTF8);
            var obj2 = formatter.Deserialize(xml, typeof(FormatModel));
            Assert.True(obj1.Equals(obj2));
        }

    }
}
