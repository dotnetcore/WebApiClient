using WebApiClient.Contexts;
using Xunit;

namespace WebApiClient.Test.Contexts
{
    public class ApiParameterDescriptorTest
    {
        [Fact]
        public void NewTest()
        {
            var p1 = typeof(IUserApi).GetMethod("Get2").GetParameters()[0];
            var m1 = new ApiParameterDescriptor(p1);
            Assert.True(m1.Attributes.Count == 1);
            Assert.True(m1.Index == 0);
            Assert.True(m1.Member == p1);
            Assert.True(m1.Name == p1.Name);
            Assert.True(m1.ParameterType == p1.ParameterType);
            Assert.True(m1.ValidationAttributes.Count == 1);
            Assert.True(m1.Value == null);


            var p2 = typeof(IUserApi).GetMethod("Get2").GetParameters()[1];
            var m2 = new ApiParameterDescriptor(p2);
            Assert.True(m2.Attributes.Count == 1);
            Assert.True(m2.Index == 1);
            Assert.True(m2.Member == p2);
            Assert.True(m2.Name == p2.Name);
            Assert.True(m2.ParameterType == p2.ParameterType);
            Assert.True(m2.ValidationAttributes.Count == 0);
            Assert.True(m2.Value == null);
        }


        [Fact]
        public void CloneTest()
        {
            var p1 = typeof(IUserApi).GetMethod("Get2").GetParameters()[0];
            var m1 = new ApiParameterDescriptor(p1);
            var m2 = m1.Clone(10);

            Assert.True(m1.Attributes == m2.Attributes);
            Assert.True(m1.Name == m2.Name);
            Assert.True(m1.Index == m2.Index);
            Assert.True(m1.ParameterType == m2.ParameterType);
            Assert.True(m1.Member == m2.Member);
            Assert.True(m1.ValidationAttributes == m2.ValidationAttributes);
            Assert.True((int)m2.Value == 10);
        }
    }
}
