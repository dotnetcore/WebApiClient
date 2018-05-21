using System;
using System.Collections.Generic;
using System.Text;
using WebApiClient;
using Xunit;

namespace WebApiClient.Test.Internal
{
    public class PropertyTest
    {
        class user
        {
            public string name { get; set; }

            public int age { get; set; }
        }

        [Fact]
        public void CopyPropertiesTest()
        {
            var source = new user { name = "laojiu", age = 10 };
            var target = new user { name = "laojiu1", age = 100 };

            Property.CopyProperties(source, target);
            Assert.True(target.name == "laojiu");
            Assert.True(target.age == 10);
        }
    }
}
