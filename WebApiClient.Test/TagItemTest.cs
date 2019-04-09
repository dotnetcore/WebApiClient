using System;
using Xunit;
using WebApiClient;

namespace WebApiClient.Test
{
    public class TagItemTest
    {
        [Fact]
        public void Test_Tag_Object()
        {
            var tag = new TagItem(new object());
            Assert.True(tag.IsNullValue == false);

            var tagNull = new TagItem(null);
            Assert.True(tagNull.IsNullValue);
        }

        [Fact]
        public void Test_Tag_Int()
        {
            var tag = new TagItem(30);
            var value = tag.AsInt32();
            Assert.True(value == 30);
        }

        [Fact]
        public void Test_Tag_Nullable()
        {
            DateTime? datetime = null;
            var tagNull = new TagItem(datetime);

            DateTime dateTime2 = DateTime.Now;
            var tagTime = new TagItem(dateTime2);

            Assert.True(tagNull.IsNullValue);
            Assert.True(tagTime.AsDateTime() == dateTime2);

            Assert.True(tagTime.ToString() == dateTime2.ToString());
        }
    }
}
