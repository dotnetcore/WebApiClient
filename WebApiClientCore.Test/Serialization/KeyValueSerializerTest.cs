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

            Assert.Equal(2, kvs.Count);
            Assert.Equal("lao九", kvs["Name"]);
            Assert.Equal("18", kvs["Age"]);


            kvs = KeyValueSerializer.Serialize("pName", 30, null)
               .ToDictionary(item => item.Key, item => item.Value);

            Assert.Single(kvs);
            Assert.Equal("30", kvs["pName"]);

            var bools = KeyValueSerializer.Serialize("bool", true, null);
            Assert.Equal("true", bools[0].Value);

            var strings = KeyValueSerializer.Serialize("strings", "\r\n", null);
            Assert.Equal("\r\n", strings[0].Value);

            var floats = KeyValueSerializer.Serialize("floats", 3.14f, null);
            Assert.Equal("3.14", floats[0].Value);

            var dic = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();
            dic.TryAdd("Key", "Value");

            var kvs2 = KeyValueSerializer.Serialize("dic", dic, options);
            Assert.Equal("key", kvs2.First().Key);


            Assert.True(KeyValueSerializer.Serialize("null", null, null).Any());
        }


        [Fact]
        public void IgnoreNullValuesTest()
        {
            var obj1 = new FormatModel { Age = 18 };
            var options = new HttpApiOptions().KeyValueSerializeOptions;
            options.IgnoreNullValues = true;

            var kvs = KeyValueSerializer.Serialize("pName", obj1, options)
               .ToDictionary(item => item.Key, item => item.Value, StringComparer.OrdinalIgnoreCase);

            Assert.False(kvs.ContainsKey("name"));
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
