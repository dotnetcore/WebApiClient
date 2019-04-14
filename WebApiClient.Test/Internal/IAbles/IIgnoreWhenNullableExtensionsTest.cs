using System;
using System.Collections.Generic;
using System.Text;
using WebApiClient;
using Xunit;
using System.Linq;
using WebApiClient.Contexts;

namespace WebApiClient.Test.Internal.IAbles
{
    public class IIgnoreWhenNullableExtensionsTest
    {
        [Fact]
        public void IsIgnoreWithTest()
        {
            object value = null;
            var ableFalse = new IgnoreWhenNullable(false);
            Assert.False(ableFalse.IgnoreWhenNull(value));

            value = 1;
            Assert.False(ableFalse.IgnoreWhenNull(value));

            var parameter = TestParameter.Create(null);
            Assert.False(ableFalse.IgnoreWhenNull(parameter));

            parameter = TestParameter.Create("laojiu");
            Assert.False(ableFalse.IgnoreWhenNull(parameter));



            value = null;
            var ableTrue = new IgnoreWhenNullable(true);
            Assert.True(ableTrue.IgnoreWhenNull(value));

            value = 1;
            Assert.False(ableTrue.IgnoreWhenNull(value));

            parameter = TestParameter.Create(null);
            Assert.True(ableTrue.IgnoreWhenNull(parameter));

            parameter = TestParameter.Create("laojiu");
            Assert.False(ableTrue.IgnoreWhenNull(parameter));
        }

        private class IgnoreWhenNullable : IIgnoreWhenNullable
        {
            public bool IgnoreWhenNull { get; set; }

            public IgnoreWhenNullable(bool ignoreWhenNull)
            {
                this.IgnoreWhenNull = ignoreWhenNull;
            }
        }
    }
}
