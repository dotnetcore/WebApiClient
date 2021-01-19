using System;
using System.Linq;
using WebApiClientCore.Serialization;
using Xunit;


namespace WebApiClientCore.Test.Serialization
{
    public class KeyValueSerializerTest
    {
        [Fact]
        public void SerializeTest()
        {
            var obj1 = new FormatModel { Age = 18, Name = "lao九" };
           
            var options = new HttpApiOptions().KeyValueSerializeOptions;

            var kvs = KeyValueSerializer.Serialize("pName", obj1, options)
                .ToDictionary(item => item.Key, item => item.Value, StringComparer.OrdinalIgnoreCase);

            Assert.True(kvs.Count == 2);
            Assert.True(kvs["Name"] == "lao九");
            Assert.True(kvs["Age"] == "18");


            kvs = KeyValueSerializer.Serialize("pName", 30, null)
               .ToDictionary(item => item.Key, item => item.Value);

            Assert.True(kvs.Count == 1);
            Assert.True(kvs["pName"] == "30");

            var bools = KeyValueSerializer.Serialize("bool", true, null);
            Assert.Equal("true", bools[0].Value);

            var strings = KeyValueSerializer.Serialize("strings", "string", null);
            Assert.Equal("string", strings[0].Value);


            var dic = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();
            dic.TryAdd("Key", "Value");

            var kvs2 = KeyValueSerializer.Serialize("dic", dic, options);
            Assert.True(kvs2.First().Key == "key");


            Assert.True(KeyValueSerializer.Serialize("null", null, null).Any());
        }

        [Fact]
        public void KeyNamingStyleTest()
        {             
            var model = new { x = new { y = 1 } };

            var value = KeyValueSerializer.Serialize("root", model, null).First();
            Assert.Equal("y", value.Key);

            value = KeyValueSerializer.Serialize("root", model, new KeyValueSerializerOptions
            {
                KeyNamingStyle = KeyNamingStyle.FullName
            }).First();
            Assert.Equal("x.y", value.Key);


            value = KeyValueSerializer.Serialize("root", model, new KeyValueSerializerOptions
            {
                KeyNamingStyle = KeyNamingStyle.FullName,
                KeyDelimiter = "|"
            }).First();
            Assert.Equal("x|y", value.Key);


            value = KeyValueSerializer.Serialize("root", model, new KeyValueSerializerOptions
            {
                KeyNamingStyle = KeyNamingStyle.FullNameWithRoot,
                KeyDelimiter = "-"
            }).First();
            Assert.Equal("root-x-y", value.Key);
        }

        [Fact]
        public void ArrayIndexFormatTest()
        {
            
            var model = new { x = new { y = new[] { 1, 2 } } };

            var kv = KeyValueSerializer.Serialize("root", model, new KeyValueSerializerOptions
            {
                KeyNamingStyle = KeyNamingStyle.FullName
            }).First();
            Assert.Equal("x.y[0]", kv.Key);
            Assert.Equal("1", kv.Value);

            kv = KeyValueSerializer.Serialize("root", model, new KeyValueSerializerOptions
            {
                KeyNamingStyle = KeyNamingStyle.FullName,
                KeyArrayIndex = (i) => $"({i})"
            }).First();
            Assert.Equal("x.y(0)", kv.Key);
            Assert.Equal("1", kv.Value);
        }
    }
}
