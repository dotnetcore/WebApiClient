using System;
using System.Linq;
using Xunit;

namespace WebApiClientCore.Test.BuildinExtensions
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
        public void GetAttributesTest()
        {
            var m1 = typeof(D1).GetMethod("M1");
            var m2 = typeof(D1).GetMethod("M2");

            Assert.True(m1.GetAttributes<MyAttribute>(true).Count() == 0);
            Assert.True(m2.GetAttributes<MyAttribute>(true).Count() == 1);
        }
    }
}
