using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 表示Api请求的异步任务抽象类
    /// </summary>
    abstract class ApiTask
    {
#if NET45
        /// <summary>
        /// 完成的任务
        /// </summary>
        /// <returns></returns>
        public static readonly Task CompletedTask = Task.FromResult<object>(null);
#else
        /// <summary>
        /// 完成的任务
        /// </summary>
        /// <returns></returns>
        public static readonly Task CompletedTask = Task.CompletedTask;
#endif

        /// <summary>
        /// 获取或设置上下文创建工厂
        /// </summary>
        public Func<ApiActionContext> ContextFactory { get; set; }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        public abstract Task Execute();
    }


    /// <summary>
    /// 表示Api请求的异步任务
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    class ApiTask<TResult> : ApiTask, ITask<TResult>, ITask
    {
        /// <summary>
        /// 执行InvokeAsync
        /// 并返回其TaskAwaiter对象
        /// </summary>
        /// <returns></returns>
        public TaskAwaiter<TResult> GetAwaiter()
        {
            return this.InvokeAsync().GetAwaiter();
        }

        /// <summary>
        /// 配置用于等待的等待者
        /// </summary>
        /// <param name="continueOnCapturedContext">试图继续回夺取的原始上下文，则为 true；否则为 false</param>
        /// <returns></returns>
        public ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext)
        {
            return this.InvokeAsync().ConfigureAwait(continueOnCapturedContext);
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        public override Task Execute()
        {
            return this.InvokeAsync();
        }

        /// <summary>
        /// 创建请求任务
        /// </summary>
        /// <returns></returns>
        Task ITask.InvokeAsync()
        {
            return this.InvokeAsync();
        }

        /// <summary>
        /// 创建请求任务
        /// </summary>
        /// <returns></returns>
        public virtual async Task<TResult> InvokeAsync()
        {
            var context = this.ContextFactory.Invoke();
            try
            {
                return await context.ExecuteActionAsync<TResult>().ConfigureAwait(false);
            }
            catch (HttpApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var api = context.ApiActionDescriptor.Name;
                var message = $"执行{api}出现{ex.GetType().Name}异常";
                throw new HttpApiException(message, ex);
            }
#if !NET45 && !NET46
            finally
            {
                context.Dispose();
            }
#endif
        }
    }
}
