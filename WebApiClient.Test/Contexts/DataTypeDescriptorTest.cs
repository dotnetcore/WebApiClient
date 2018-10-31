using System.Linq;
using WebApiClient.Contexts;
using Xunit;

namespace WebApiClient.Test.Contexts
{
    public class DataTypeDescriptorTest
    {
        private DataTypeDescriptor Create(string methodName)
        {
            var method = typeof(IUserApi).GetMethod(methodName);
            var dataType = method.ReturnType.GetGenericArguments().FirstOrDefault();
            return new DataTypeDescriptor(dataType);
        }

        [Fact]
        public void NewTest()
        {
            var m1 = Create("Get1");
            var m2 = Create("Get2");
            var m3 = Create("Get3");
            var m4 = Create("Get4");
            var m5 = Create("Get5");
            var m6 = Create("Get6");

            Assert.True(m1.IsString);
            Assert.True(m2.IsHttpResponseMessage);
            Assert.True(m3.IsStream);
            Assert.True(m4.IsHttpResponseWrapper);
            Assert.True(m6.IsByteArray);
            Assert.False(m5.IsString || m5.IsStream || m5.IsByteArray || m5.IsHttpResponseMessage || m5.IsHttpResponseWrapper);
        }
    }
}
