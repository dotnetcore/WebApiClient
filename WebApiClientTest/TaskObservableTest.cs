using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiClient;
using Xunit;

namespace WebApiClientTest
{
    public class TaskObservableTest
    {
        private int count1 = 0;
        private int count2 = 0;

        [Fact]
        public async Task CompletedTaskObservableTestAsync()
        {
            var taskSource = new TaskCompletionSource<int>();

            var observable = Task.FromResult(5).ToObservable();
            await Task.Delay(20);

            observable.Subscribe(r =>
            {
                Interlocked.Increment(ref this.count1);
                taskSource.SetResult(r);
            }, null);

            var result = await taskSource.Task;
            Assert.True(this.count1 == 1);
            Assert.True(result == 5);
        }

        [Fact]
        public async Task TaskObservableTestAsync()
        {
            var taskSource = new TaskCompletionSource<int>();

            this.GetDelayTask(delay: 20, value: 5)
                .ToObservable()
                .Subscribe(r =>
                {
                    Interlocked.Increment(ref this.count2);
                    taskSource.SetResult(r);
                }, null);

            var result = await taskSource.Task;
            Assert.True(this.count2 == 1);
            Assert.True(result == 5);
        }

        [Fact]
        public async Task TaskObservableErrorTestAsync()
        {
            var taskSource = new TaskCompletionSource<int>();

            this.GetDelayTask(delay: 20, exception: new TimeoutException())
                .ToObservable()
                .Subscribe(null, ex =>
                {
                    taskSource.SetException(ex);
                });

            Exception exception = null;
            try
            {
                await taskSource.Task;
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.True(exception != null && exception is TimeoutException);
        }

        private async Task<int> GetDelayTask(int delay, Exception exception)
        {
            await Task.Delay(delay);
            throw exception;
        }

        private async Task<int> GetDelayTask(int delay, int value)
        {
            await Task.Delay(delay);
            return value;
        }
    }
}
