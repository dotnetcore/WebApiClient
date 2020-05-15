using WebApiClientCore.Attributes;
using Xunit;

namespace WebApiClientCore.Test
{
    public class ApiReturnDescriptorTest
    {
        [Fact]
        public void CtorTest()
        {
            var m1 = new ApiReturnDescriptor(typeof(IDescriptorApi).GetMethod("Get1"));
            var m2 = new ApiReturnDescriptor(typeof(IDescriptorApi).GetMethod("Get2"));
             
            Assert.True(m1.Attribute.GetType() == typeof(AutoResultAttribute));
               
            Assert.True(m2.Attribute.GetType() == typeof(JsonResultAttribute));
        }
    }
}
