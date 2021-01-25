using WebApiClientCore.Implementations;
using Xunit;

namespace WebApiClientCore.Test.Implementations
{
    public class DefaultDataCollectionTest
    {
        [Fact]
        public void ReadWriteTest()
        {
            var datas = new DefaultDataCollection();
            datas.Set("string", "a");
            datas.Set(2, 1);
            datas.Set(typeof(DefaultDataCollectionTest), new DefaultDataCollectionTest());

            Assert.True(datas.Get<string>("string") == "a");
            Assert.True(datas.Get<int>(2) == 1);

            var state = datas.TryGetValue(typeof(DefaultDataCollectionTest), out var value);
            Assert.True(state && value is DefaultDataCollectionTest);

            state = datas.TryGetValue<DefaultDataCollectionTest>(typeof(DefaultDataCollectionTest), out var tValue);
            Assert.True(state && tValue != null);

            state = datas.TryGetValue<DefaultDataCollectionTest>(typeof(string), out _);
            Assert.False(state);

            datas.TryRemove("string", out _);
            Assert.True(datas.Get<string>("string") == default);
        }
    }
}
