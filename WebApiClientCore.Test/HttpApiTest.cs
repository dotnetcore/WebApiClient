using System;
using System.Linq;
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
            Assert.Equal(3, m1.Length);

            var m3 = HttpApi.FindApiMethods(typeof(IPostNew));
            Assert.Equal(2, m3.Length);
            Assert.Contains(m3, i => i.IsDefined(typeof(NewAttribute), true));
        }
    }
}
