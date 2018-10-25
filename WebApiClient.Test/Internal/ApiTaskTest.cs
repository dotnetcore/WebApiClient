using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebApiClient.Test.Internal
{
    public class ApiTaskTest
    {
        class MyApiTask<T> : ApiTask<T>
        {
            public T Result { get; set; }

            public override Task<T> InvokeAsync()
            {
                return Task.FromResult(Result);
            }
        }

        [Fact]
        public async Task ExecuteTest()
        {
            var apiTask = new MyApiTask<string> { Result = "abc" };
            var task = apiTask.Execute();
            await task;
            var r = task as Task<string>;
            Assert.True(r.Result == apiTask.Result);
        }

        [Fact]
        public async Task InvokeAsyncTest()
        {
            var apiTask = new MyApiTask<string> { Result = "abc" };
            var task = ((ITask)apiTask).InvokeAsync();
            await task;
            var r = task as Task<string>;
            Assert.True(r.Result == apiTask.Result);
        }

        [Fact]
        public async Task InvokeAsync2Test()
        {
            var apiTask = new MyApiTask<string> { Result = "abc" };
            var r = await apiTask.InvokeAsync();
            Assert.True(r == apiTask.Result);
        }

        [Fact]
        public void GetAwaiterTest()
        {
            var apiTask = new MyApiTask<string> { Result = "abc" };
            var r = apiTask.GetAwaiter().GetResult();
            Assert.True(r == apiTask.Result);
        }

        [Fact]
        public void ConfigureAwaitTest()
        {
            var apiTask = new MyApiTask<string> { Result = "abc" };
            var r = apiTask.ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.True(r == apiTask.Result);
        }
    }
}
