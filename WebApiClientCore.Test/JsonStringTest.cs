using System.Text.Json;
using WebApiClientCore.Serialization.JsonConverters;
using Xunit;

namespace WebApiClientCore.Test
{
    public class JsonStringTest
    {
        [Fact]
        public void JsonStringReadWriteTest()
        {
            var options = new HttpApiOptions().JsonSerializeOptions;

            options.Converters.Add(JsonStringTypeConverter.Default);
            var b = new JsonString<B>(new B());
            var json1 = JsonSerializer.Serialize(b, options);
            var json2 = JsonSerializer.Serialize(b.Value, options);
            var json3 = JsonSerializer.Serialize(json2, options);

            Assert.Equal(json1, json3);

            var b2 = JsonSerializer.Deserialize(json3, typeof(JsonString<B>), options) as JsonString<B>;
            Assert.Equal("name", b2.Value.Name);


            var a = new A();
            var aJson = JsonSerializer.Serialize(a, options);
            var a2 = JsonSerializer.Deserialize(aJson, typeof(A), options) as A;

            Assert.Equal(a.Age, a2.Age);
            Assert.Equal(a.B.Value.Name, a2.B.Value.Name);
        }

        class A
        {
            public int Age { get; set; } = 10;

            public JsonString<B> B { get; set; } = new B();
        }

        class B
        {
            public string Name { get; set; } = "name";
        }
    }
}
