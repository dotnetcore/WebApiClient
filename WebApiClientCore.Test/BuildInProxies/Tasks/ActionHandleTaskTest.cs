using System;
using System.Threading.Tasks;
using Xunit;

namespace WebApiClientCore.Test.BuildInProxies.Tasks
{
    public class ActionHandleTaskTest
    {
        class NotImplementedApiTask<T> : TaskBase<T>
        {
            protected override Task<T> InvokeAsync()
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

            result = await apiTask.Handle().WhenCatchAsync<Exception>((ex) => Task.FromResult("xyz"));
            Assert.True(result == "xyz");

            await Assert.ThrowsAsync<NotImplementedException>(async () =>
                await apiTask.Handle().WhenCatchAsync<NotSupportedException>((ex) => Task.FromResult("xyz")));
        }
    }
}
