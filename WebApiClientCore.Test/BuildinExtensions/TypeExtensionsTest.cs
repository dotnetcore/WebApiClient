using System;
using System.Linq;
using System.Text.Json.Serialization;
using Xunit;

namespace WebApiClientCore.Test.BuildinExtensions
{
    public class TypeExtensionsTest
    {
        [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
        class A : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        class B : Attribute
        {
        }

        class C : B
        {
            [JsonPropertyName("arg")]
            public int Age { get; set; }

            public string? Name { get; set; }
        }

        public interface IGet
        {
            ITask<string> Get();
        }

        public interface IPost
        {
            ITask<string> Post();
        }

        public interface IMyApi : IGet, IPost
        {
            ITask<int> Delete();
        }

        [Fact]
        public void AllowMultipleTest()
        {
            Assert.False(typeof(A).IsAllowMultiple());
            Assert.True(typeof(B).IsAllowMultiple());
            Assert.True(typeof(C).IsAllowMultiple());
        }

        [Fact]
        public void DefaultValueTest()
        {
            Assert.Null(typeof(A).DefaultValue());
            Assert.Null(typeof(int?).DefaultValue());
            Assert.Equal(0, (int?)typeof(int).DefaultValue());
            Assert.Equal(0f, (float?)typeof(float).DefaultValue());
            Assert.True((Guid?)typeof(Guid).DefaultValue() == Guid.Empty);
        }

        [Fact]
        public void IsInheritFromTest()
        {
            Assert.True(typeof(C).IsInheritFrom<B>());
            Assert.False(typeof(B).IsInheritFrom<A>());
        }


        class MyAttribute : Attribute
        {
        }

        class YourAttribute : Attribute
        {
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
        public void GetInterfaceCustomAttributesTest()
        {
            Assert.Equal(2, typeof(Interface2).GetInterfaceCustomAttributes().Length);
            Assert.Equal(4, typeof(Interface3).GetInterfaceCustomAttributes().Length);
        }
    }
}
