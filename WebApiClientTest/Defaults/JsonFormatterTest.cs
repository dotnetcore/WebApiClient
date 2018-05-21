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
        }
    }
}
