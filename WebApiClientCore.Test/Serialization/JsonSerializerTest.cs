using System.Text;
using Xunit;

namespace WebApiClientCore.Test.Serialization
{
    public class JsonSerializerTest
    {
        [Fact]
        public void ReadWriteTest()
        {
            var options = new HttpApiOptions().JsonSerializeOptions;

            var obj1 = new FormatModel { Age = 18, Name = "老九" };
            var formatter = new WebApiClientCore.Serialization.JsonSerializer();
            using var buffer = new BufferWriter<byte>();
            formatter.Serialize(buffer, obj1, options);
            var json = buffer.GetWrittenSpan().ToArray();
            var obj2 = formatter.Deserialize(json, typeof(FormatModel), options);
            Assert.True(obj1.Equals(obj2));

            var dic = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();
            dic.TryAdd("Key", "Value");

            buffer.Clear();

            formatter.Serialize(buffer,dic, options);
            var json2 = Encoding.UTF8.GetString(buffer.GetWrittenSpan().ToArray());
            Assert.Contains("key", json2);
        }
    }
}
