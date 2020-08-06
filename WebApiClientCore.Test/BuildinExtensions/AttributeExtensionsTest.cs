using System;
using System.Linq;
using Xunit;

namespace WebApiClientCore.Test.BuildinExtensions
{
    public class AttributeExtensionsTest
    {

        class MyAttribute : Attribute
        {
        }

        class YourAttribute : Attribute
        {
        }

        class Class1
        {
            public void M1()
            {
            }

            [My]
            public void M2()
            {
            }
        }

        [My]
        interface Inteface1
        {
        }

        [My]
        interface Interface2 : Inteface1
        {
        }

        [Your]
        [My]
        interface Interface3 : Interface2
        {
        }



        [Fact]
        public void GetAttributesTest()
        {
            var m1 = typeof(Class1).GetMethod("M1");
            var m2 = typeof(Class1).GetMethod("M2");

            Assert.True(m1.GetAttributes<MyAttribute>().Count() == 0);
            Assert.True(m2.GetAttributes<MyAttribute>().Count() == 1);

            Assert.Equal(2, typeof(Interface2).GetAttributes<Attribute>(inclueBases: true).Count());
            Assert.Equal(4, typeof(Interface3).GetAttributes<Attribute>(inclueBases: true).Count());
        }
    }
}
