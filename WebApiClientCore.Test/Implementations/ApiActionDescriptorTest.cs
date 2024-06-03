using WebApiClientCore.Implementations;
using Xunit;

namespace WebApiClientCore.Test.Implementations
{
    public class ApiActionDescriptorTest
    {
        [Fact]
        public void CtorTest()
        {
            var m = typeof(IDescriptorApi).GetMethod("Get1");
            Assert.NotNull(m);
            var d = new DefaultApiActionDescriptor(m);

            Assert.Equal(3, d.Attributes.Count);
            Assert.Single(d.FilterAttributes);
            Assert.Equal(2, d.Parameters.Count);
            Assert.True(d.Name == m.Name);
            Assert.True(d.Member == m);
            Assert.Single(d.Return.Attributes);
            Assert.True(d.Return.ReturnType == m.ReturnType);
        }
    }
}
