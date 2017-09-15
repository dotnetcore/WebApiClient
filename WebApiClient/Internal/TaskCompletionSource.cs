using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 提供任务的创建
    /// </summary>
    class TaskCompletionSource
    {
        /// <summary>
        /// 提供任务的创建接口
        /// </summary>
        private readonly ITaskSource taskSource;

        /// <summary>
        /// 获取task对象
        /// </summary>
        public Task Task
        {
            get
            {
                return this.taskSource.Task;
            }
        }

        /// <summary>
        /// 任务的创建
        /// </summary>
        /// <param name="resultType">result类型</param>
        public TaskCompletionSource(Type resultType)
        {
            var type = typeof(TaskSource<>).MakeGenericType(resultType);
            this.taskSource = Activator.CreateInstance(type) as ITaskSource;
        }

        /// <summary>
        /// 设置结果
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool SetResult(object result)
        {
            return this.taskSource.SetResult(result);
        }

        /// <summary>
        /// 设置异常
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool SetException(Exception ex)
        {
            return this.taskSource.SetException(ex);
        }

        /// <summary>
        /// 提供任务的创建接口
        /// </summary>
        private interface ITaskSource
        {
            /// <summary>
            /// 获取task对象
            /// </summary>
            Task Task { get; }

            /// <summary>
            /// 设置结果
            /// </summary>
            /// <param name="result"></param>
            /// <returns></returns>
            bool SetResult(object result);

            /// <summary>
            /// 设置异常
            /// </summary>
            /// <param name="ex"></param>
            /// <returns></returns>
            bool SetException(Exception ex);
        }


        /// <summary>
        /// 提供Task结果设置
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        private class TaskSource<TResult> : TaskCompletionSource<TResult>, ITaskSource
        {
            /// <summary>
            /// 获取task对象
            /// </summary>
            Task ITaskSource.Task
            {
                get
                {
                    return base.Task;
                }
            }

            /// <summary>
            /// 设置结果
            /// </summary>
            /// <param name="result"></param>
            /// <returns></returns>
            bool ITaskSource.SetResult(object result)
            {
                return base.TrySetResult((TResult)result);
            }

            /// <summary>
            /// 设置异常
            /// </summary>
            /// <param name="ex"></param>
            /// <returns></returns>
            bool ITaskSource.SetException(Exception ex)
            {
                return base.TrySetException(ex);
            }
        }
    }
}
