using WebApiClient;
using Xunit;

namespace WebApiClientTest.Internal
{
    public class ApiDescriptorCacheTest
    {
        public interface IMyApi : IHttpApiClient
        {
            ITask<string> Login(int a);
        }

        [Fact]
        public void GetApiActionDescriptorTest()
        {
            var method = typeof(IMyApi).GetMethod("Login");
            var descriptor1 = ApiDescriptorCache.GetApiActionDescriptor(method);
            var descriptor2 = ApiDescriptorCache.GetApiActionDescriptor(method);
            Assert.True(object.ReferenceEquals(descriptor1, descriptor2));

            Assert.True(descriptor1.Name == "Login");
            Assert.True(descriptor1.Parameters.Length == 1);
            Assert.True(descriptor1.Filters.Length == 0);
            Assert.True(descriptor1.Attributes.Length == 0);
        }
    }
}
