using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebApiClient.Test.Internal.Tasks
{
    public class ApiRetryTaskTest
    {
        class ResultApiTask<T> : ApiTask<T>
        {
            public T Result { get; set; }

            public override Task<T> InvokeAsync()
            {
                return Task.FromResult(Result);
            }
        }

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
