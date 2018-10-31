using WebApiClient.Contexts;
using Xunit;

namespace WebApiClient.Test.Contexts
{
    public class ApiActionDescriptorTest
    {
        [Fact]
        public void NewTest()
        {
            var m = typeof(IUserApi).GetMethod("Get1");
            var d = new ApiActionDescriptor(m);

            Assert.True(d.Attributes.Count == 3);
            Assert.True(d.Filters.Count == 1);
            Assert.True(d.Parameters.Count == 2);
            Assert.True(d.Name == m.Name);
            Assert.True(d.Member == m);
            Assert.True(d.Return.ReturnType == m.ReturnType);
        }

        [Fact]
        public void CloneTest()
        {
            var m = typeof(IUserApi).GetMethod("Get1");
            var d = new ApiActionDescriptor(m);
            var d2 = d.Clone(new object[] { null, null });

            Assert.True(d.Attributes == d2.Attributes);
            Assert.True(d.Name == d2.Name);
            Assert.True(d.Filters == d2.Filters);
            Assert.True(d.Member == d2.Member);
            Assert.True(d.Parameters != d2.Parameters);
            Assert.True(d.Parameters.Count == d2.Parameters.Count);
            Assert.True(d.Return == d2.Return);
        }
    }
}
