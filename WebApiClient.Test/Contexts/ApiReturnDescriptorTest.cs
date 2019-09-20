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

            Assert.Null(m2.ReturnValueMapper);
            Assert.Equal(m2.ReturnDataType, m2.DataType.Type);
        }

        [Fact]
        public void ReturnValueMapper()
        {
            var m7 = new ApiReturnDescriptor(typeof(IUserApi).GetMethod("Get7"));

            Assert.NotNull(m7.ReturnDataType);
            Assert.Equal(typeof(string), m7.ReturnDataType);
            Assert.Equal(typeof(byte[]), m7.DataType.Type);

            Assert.NotNull(m7.ReturnValueMapper);
            Assert.IsType<TestReturnValueMapper>(m7.ReturnValueMapper);

        }
    }
}
