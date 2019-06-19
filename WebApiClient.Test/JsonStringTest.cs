using Xunit;

namespace WebApiClient.Test
{
    public class JsonStringTest
    {
        [Fact]
        public void Test_Tag_Object()
        {
            var formatter = WebApiClient.Defaults.JsonFormatter.Instance;
            var b = new JsonString<B>(new B());
            var json1 = formatter.Serialize(b, null);
            var json2 = formatter.Serialize(b.Value, null);
            var json3 = formatter.Serialize(json2, null);
            Assert.Equal(json1, json3);

            var b2 = formatter.Deserialize(json3, typeof(JsonString<B>)) as JsonString<B>;
            Assert.Equal("name", b2.Value.Name);


            var a = new A();
            var aJson = formatter.Serialize(a, null);
            var a2 = formatter.Deserialize(aJson, typeof(A)) as A;

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
