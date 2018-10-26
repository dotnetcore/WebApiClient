using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace WebApiClient.Test.Internal
{
    public class Guid16Test
    {
        [Fact]
        public void ToInt64Test()
        {
            var guid16 = new Guid16(5);
            Assert.Equal(5L, guid16.ToInt64());
        }

        [Fact]
        public void EmptyTest()
        {
            Assert.True(Guid16.Empty.ToInt64() == new Guid16().ToInt64());
        }

        [Fact]
        public void ParseTest()
        {
            var guid16 = new Guid16(5L);
            var g = Guid16.Parse(guid16.ToString());
            Assert.Equal(guid16, g);
        }

        [Fact]
        public void EqualTest()
        {
            var g = new Guid16(5L);
            var g1 = new Guid16(5L);
            object g2 = new Guid16(5L);

            Assert.True(g.Equals(g2));
            Assert.True(g.Equals(g2));
        }

        [Fact]
        public void CompareToTest()
        {
            var g = new Guid16(5L);
            var g1 = new Guid16(5L);
            Assert.True(g.CompareTo(g1) == 0);
        }

        [Fact]
        public void ToStringTest()
        {
            var g = new Guid16(5L);
            Assert.Equal(g.ToString(), "5".PadLeft(16, '0'));
        }
    }
}
