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
            var descriptor1 = new ApiActionDescriptor(method);
            var descriptor2 = new ApiActionDescriptor(method);
            Assert.False(object.ReferenceEquals(descriptor1, descriptor2));

            Assert.True(descriptor1.Name == "Login");
            Assert.True(descriptor1.Parameters.Count == 1);
            Assert.True(descriptor1.Filters.Count == 0);
            Assert.True(descriptor1.Attributes.Count == 0);
        }
    }
}
