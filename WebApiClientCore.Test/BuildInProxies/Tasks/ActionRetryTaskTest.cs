using System;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;
using Xunit;

namespace WebApiClientCore.Test.BuildInProxies.Tasks
{
    public class ActionRetryTaskTest
    {
        class ResultApiTask<T> : TaskBase<T>
        {
            public T Result { get; set; }

            protected override Task<T> InvokeAsync()
            {
                return Task.FromResult(Result);
            }
        }

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
            await Assert.ThrowsAsync<ApiRetryException>(async () =>
                await apiTask.Retry(3).WhenCatch<NotImplementedException>());
        }

        [Fact]
        public async Task WhenCatchAsyncTest()
        {
            var apiTask = new NotImplementedApiTask<string>();
            await Assert.ThrowsAsync<ApiRetryException>(async () =>
                await apiTask.Retry(3).WhenCatchAsync<NotImplementedException>(async ex => await Task.CompletedTask));
        }


        [Fact]
        public async Task WhenResultTest()
        {
            var apiTask = new ResultApiTask<string> { Result = "abc" };
            await apiTask.Retry(3).WhenResult(r => r == null);

            await Assert.ThrowsAsync<ApiRetryException>(async () =>
                await apiTask.Retry(3).WhenResult(r => r == apiTask.Result));
        }

        [Fact]
        public async Task WhenResultAsyncTest()
        {
            var apiTask = new ResultApiTask<string> { Result = "abc" };
            await Assert.ThrowsAsync<ApiRetryException>(async () =>
                await apiTask.Retry(3).WhenResultAsync(r => Task.FromResult(r == apiTask.Result)));
        }
    }
}
