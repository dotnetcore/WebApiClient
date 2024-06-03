using WebApiClientCore.Implementations;
using Xunit;

namespace WebApiClientCore.Test.Implementations
{
    public class ApiParameterDescriptorTest
    {
        [Fact]
        public void CtorTest()
        {
            var p1 = typeof(IDescriptorApi).GetMethod("Get2")!.GetParameters()[0];
            var m1 = new DefaultApiParameterDescriptor(p1);
            Assert.Single(m1.Attributes);
            Assert.Equal(0, m1.Index);
            Assert.True(m1.Member == p1);
            Assert.True(m1.Name == p1.Name);
            Assert.True(m1.ParameterType == p1.ParameterType);
            Assert.Single(m1.ValidationAttributes);


            var p2 = typeof(IDescriptorApi).GetMethod("Get2")!.GetParameters()[1];
            var m2 = new DefaultApiParameterDescriptor(p2);
            Assert.Single(m2.Attributes);
            Assert.Equal(1, m2.Index);
            Assert.True(m2.Member == p2);
            Assert.True(m2.Name == p2.Name);
            Assert.True(m2.ParameterType == p2.ParameterType);
            Assert.Empty(m2.ValidationAttributes);
        }
    }
}
