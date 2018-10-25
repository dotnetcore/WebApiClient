using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebApiClient.Test.Internal
{
    public class ApiHandleTaskTest
    {
        class NotImplementedApiTask<T> : ApiTask<T>
        {
            public override Task<T> InvokeAsync()
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public async Task WhenCatchTest()
        {
            var apiTask = new NotImplementedApiTask<string>();
            var result = await apiTask.Handle().WhenCatch<NotImplementedException>(() => "abc");
            Assert.True(result == "abc");

            result = await apiTask.Handle().WhenCatch<Exception>((ex) => "xyz");
            Assert.True(result == "xyz");

            await Assert.ThrowsAsync<NotImplementedException>(async () =>
               await apiTask.Handle().WhenCatch<NotSupportedException>(() => "xyz"));
        }
    }
}
