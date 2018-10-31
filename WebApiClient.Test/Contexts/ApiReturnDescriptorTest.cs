using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;

namespace WebApiClient.Test.Contexts
{
    public class ApiReturnDescriptorTest
    {
        [Fact]
        public void NewTest()
        {
            var m1 = new ApiReturnDescriptor(typeof(IUserApi).GetMethod("Get1"));
            var m2 = new ApiReturnDescriptor(typeof(IUserApi).GetMethod("Get2"));

            Assert.True(m1.IsTaskDefinition);
            Assert.True(m1.Attribute.GetType() == typeof(AutoReturnAttribute));

            Assert.True(m2.IsITaskDefinition);    
            Assert.True(m2.Attribute.GetType() == typeof(JsonReturnAttribute));
        }
    }
}
