using System;
using Xunit;

namespace WebApiClientCore.Test
{
    public class HttpApiTest
    {

        public class NewAttribute : Attribute
        {

        }

        public interface IGet
        {
            ITask<string> Get();
        }

        public interface IPost
        {
            ITask<string> Post();
        }

        public interface IPostNew : IPost
        {
            [New]
            new ITask<string> Post();
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

            var m3 = HttpApi.FindApiMethods(typeof(IPostNew));
            Assert.Single(m3);
            Assert.True(m3[0].IsDefined(typeof(NewAttribute), true));
        }
    }
}
