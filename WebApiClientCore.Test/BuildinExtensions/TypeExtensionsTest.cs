using System;
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

            public string Name { get; set; }
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
            Assert.True(typeof(A).DefaultValue() == null);
            Assert.True(typeof(int?).DefaultValue() == null);
            Assert.True((int)typeof(int).DefaultValue() == 0);
            Assert.True((float)typeof(float).DefaultValue() == 0f);
            Assert.True((Guid)typeof(Guid).DefaultValue() == Guid.Empty);
        }

        [Fact]
        public void IsInheritFromTest()
        {
            Assert.True(typeof(C).IsInheritFrom<B>());
            Assert.False(typeof(B).IsInheritFrom<A>());
        }


        [Fact]
        public void GetAllApiMethodsTest()
        {
            var m1 = typeof(IMyApi).GetAllApiMethods();
            var m2 = typeof(IMyApi).GetAllApiMethods();

            Assert.False(object.ReferenceEquals(m1, m2));
            Assert.True(m1.Length == 3);
        }
    }
}
