using System;
using System.Linq;
using WebApiClient;
using Xunit;

namespace WebApiClient.Test.Internal
{
    public class AttributeExtensionsTest
    {
        interface IAttribute
        {
        }

        class MyAttribute : Attribute, IAttribute
        {
        }

        class YourAttribute : Attribute, IAttribute
        {
        }

        class MyClass
        {
            [My]
            public int Age { get; set; }

            [My, Your]
            public string Name { get; set; }

            public void Set(int age, [My, Your] string name)
            {
            }
        }

        class D1
        {
            public void M1()
            {
            }

            [My]
            public void M2()
            {
            }
        }         

        [Fact]
        public void FindDeclaringAttributeTest()
        {
            var m1 = typeof(D1).GetMethod("M1");
            var m2 = typeof(D1).GetMethod("M2");
            Assert.Null(m1.FindDeclaringAttribute<MyAttribute>(true));
            Assert.NotNull(m2.FindDeclaringAttribute<MyAttribute>(true));
        }

        [Fact]
        public void FindDeclaringAttributesTest()
        {
            var m1 = typeof(D1).GetMethod("M1");
            var m2 = typeof(D1).GetMethod("M2");

            Assert.True(m1.FindDeclaringAttributes<MyAttribute>(true).Count() == 0);
            Assert.True(m2.FindDeclaringAttributes<MyAttribute>(true).Count() == 1);
        }
    }
}
