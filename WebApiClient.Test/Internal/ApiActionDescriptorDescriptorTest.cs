using WebApiClient;
using WebApiClient.Contexts;
using Xunit;

namespace WebApiClient.Test.Internal
{
    public class ApiActionDescriptorDescriptorTest
    {
        public interface IMyApi : IHttpApi
        {
            ITask<string> Login(int a);
        }

        [Fact]
        public void GetApiActionDescriptorTest()
        {
            var method = typeof(IMyApi).GetMethod("Login");
            var descriptor1 = ApiActionDescriptor.Create(method);
            var descriptor2 = ApiActionDescriptor.Create(method);
            Assert.False(object.ReferenceEquals(descriptor1, descriptor2));

            Assert.True(descriptor1.Name == "Login");
            Assert.True(descriptor1.Parameters.Length == 1);
            Assert.True(descriptor1.Filters.Length == 0);
            Assert.True(descriptor1.Attributes.Length == 0);
        }
    }
}
