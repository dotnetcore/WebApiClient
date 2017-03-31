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
        /// <param name="taskResult">Taskof(Object)</param>
        /// <param name="resultType">Object对应的强类型</param>
        /// <returns></returns>
        public static Task CastResult(this Task<object> taskResult, Type resultType)
        {
            var taskSource = new TaskCompletionSource(resultType);
            taskResult.ContinueWith((task) =>
            {
                try
                {
                    taskSource.SetResult(task.Result);
                }
                catch (AggregateException ex)
                {
                    taskSource.SetException(ex.InnerException);
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