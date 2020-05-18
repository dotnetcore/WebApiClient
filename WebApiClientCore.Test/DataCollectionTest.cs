using Xunit;

namespace WebApiClientCore.Test
{
    public class DataCollectionTest
    {
        [Fact]
        public void ReadWriteTest()
        {
            var datas = new DataCollection();
            datas.Set("string", "a");
            datas.Set(2, 1);
            datas.Set(typeof(DataCollectionTest), new DataCollectionTest());

            Assert.True(datas.Get<string>("string") == "a");
            Assert.True(datas.Get<int>(2) == 1);

            var state = datas.TryGetValue(typeof(DataCollectionTest), out var value);
            Assert.True(state && value is DataCollectionTest);

            datas.TryRemove("string", out _);
            Assert.True(datas.Get<string>("string") == default);
        }
    }
}
