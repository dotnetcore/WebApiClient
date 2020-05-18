using System;
using Xunit;

namespace WebApiClientCore.Test
{
    public class KeyValueTest
    {
        [Fact]
        public void CtorTest()
        {
            Assert.Throws<ArgumentNullException>(() => new KeyValue("", ""));
            Assert.Throws<ArgumentNullException>(() => new KeyValue(null, ""));

            var kv = new KeyValue("key", "value");
            Assert.Equal("key", kv.Key);
            Assert.Equal("value", kv.Value);
        }
    }
}
