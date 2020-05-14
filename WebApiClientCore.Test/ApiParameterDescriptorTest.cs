using Xunit;

namespace WebApiClientCore.Test
{
    public class ApiParameterDescriptorTest
    {
        [Fact]
        public void CtorTest()
        {
            var p1 = typeof(IDescriptorApi).GetMethod("Get2").GetParameters()[0];
            var m1 = new ApiParameterDescriptor(p1);
            Assert.True(m1.Attributes.Count == 1);
            Assert.True(m1.Index == 0);
            Assert.True(m1.Member == p1);
            Assert.True(m1.Name == p1.Name);
            Assert.True(m1.ParameterType == p1.ParameterType);
            Assert.True(m1.ValidationAttributes.Count == 1);


            var p2 = typeof(IDescriptorApi).GetMethod("Get2").GetParameters()[1];
            var m2 = new ApiParameterDescriptor(p2);
            Assert.True(m2.Attributes.Count == 1);
            Assert.True(m2.Index == 1);
            Assert.True(m2.Member == p2);
            Assert.True(m2.Name == p2.Name);
            Assert.True(m2.ParameterType == p2.ParameterType);
            Assert.True(m2.ValidationAttributes.Count == 0);
        }
    }
}
