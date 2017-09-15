using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 提供Taskof(Object)类型转换强类型
    /// </summary>
    static class TaskExtend
    {
        /// <summary>
        /// 完成的任务
        /// </summary>
        /// <returns></returns>
        public static readonly Task CompletedTask = Task.FromResult<object>(null);

        /// <summary>
        /// 转换Taskof(Object)的Result转换为resultType类型
        /// </summary>
        /// <param name="task">Taskof(Object)</param>
        /// <param name="resultType">Object对应的强类型</param>
        /// <returns></returns>
        public static Task CastResult(this Task<object> task, Type resultType)
        {
            var awaiter = task.GetAwaiter();
            var taskSource = new TaskCompletionSource(resultType);

            awaiter.OnCompleted(() =>
            {
                try
                {
                    taskSource.SetResult(awaiter.GetResult());
                }
                catch (Exception ex)
                {
                    taskSource.SetException(ex);
                }
            });
            return taskSource.Task;
        }
    }
}