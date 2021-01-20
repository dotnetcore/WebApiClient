using Xunit;

namespace WebApiClientCore.Test
{
    public class HttpApiTest
    {


        public interface IGet
        {
            ITask<string> Get();
        }

        public interface IPost
        {
            ITask<string> Post();
        }

        public interface IMyApi : IGet, IPost
        {
            ITask<int> Delete();
        }

        [Fact]
        public void GetAllApiMethodsTest()
        {
            var m1 = HttpApi.FindApiMethods(typeof(IMyApi));
            var m2 = HttpApi.FindApiMethods(typeof(IMyApi));

            Assert.False(object.ReferenceEquals(m1, m2));
            Assert.True(m1.Length == 3);
        }
    }
}
