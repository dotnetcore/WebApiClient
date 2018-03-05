using System;
using System.Linq;
using WebApiClient;
using Xunit;

namespace WebApiClientTest.Internal
{
    public class AttributeExtendTest
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
        public void GetAttributeTest()
        {
            var age = typeof(MyClass).GetProperty("Age");
            Assert.NotNull(age.GetAttribute<IAttribute>(true));
            Assert.NotNull(age.GetAttribute<MyAttribute>(true));
        }

        [Fact]
        public void GetAttributesTest()
        {
            var age = typeof(MyClass).GetProperty("Age");
            Assert.True(age.GetAttributes<MyAttribute>(true).Count() == 1);

            var name = typeof(MyClass).GetProperty("Name");
            Assert.True(name.GetAttributes<IAttribute>(true).Count() == 2);

            var set = typeof(MyClass).GetMethod("Set");
            var p1 = set.GetParameters().First();
            var p2 = set.GetParameters().Last();

            Assert.True(p1.GetAttributes<IAttribute>(true).Count() == 0);
            Assert.True(p2.GetAttributes<IAttribute>(true).Count() == 2);
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
