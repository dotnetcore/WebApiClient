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
            Assert.False(ableFalse.IsIgnoreWith(value));

            value = 1;
            Assert.False(ableFalse.IsIgnoreWith(value));

            var parameter = new ApiParameterDescriptor { Value = null };
            Assert.False(ableFalse.IsIgnoreWith(parameter));

            parameter = new ApiParameterDescriptor { Value = "laojiu" };
            Assert.False(ableFalse.IsIgnoreWith(parameter));



            value = null;
            var ableTrue = new IgnoreWhenNullable(true);
            Assert.True(ableTrue.IsIgnoreWith(value));

            value = 1;
            Assert.False(ableTrue.IsIgnoreWith(value));

            parameter = new ApiParameterDescriptor { Value = null };
            Assert.True(ableTrue.IsIgnoreWith(parameter));

            parameter = new ApiParameterDescriptor { Value = "laojiu" };
            Assert.False(ableTrue.IsIgnoreWith(parameter));
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
