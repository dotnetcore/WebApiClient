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
    static class TaskConvter
    {
        /// <summary>
        /// 转换Taskof(Object)为强类型
        /// </summary>
        /// <param name="taskResult">Taskof(Object)</param>
        /// <param name="resultType">Object对应的强类型</param>
        /// <returns></returns>
        public static Task Cast(Task<object> taskResult, Type resultType)
        {
            var taskSetterType = typeof(TaskSetter<>).MakeGenericType(resultType);
            var taskSetter = Activator.CreateInstance(taskSetterType) as ITaskSetter;

            taskResult.ContinueWith((task) =>
            {
                try
                {
                    taskSetter.SetResult(task.Result);
                }
                catch (AggregateException ex)
                {
                    taskSetter.SetException(ex.InnerException);
                }
                catch (Exception ex)
                {
                    taskSetter.SetException(ex);
                }
            });

            return taskSetter.Task;
        }


        /// <summary>
        /// Task结果设置
        /// </summary>
        private interface ITaskSetter
        {
            /// <summary>
            /// 获取task对象
            /// </summary>
            Task Task { get; }

            /// <summary>
            /// 设置结果
            /// </summary>
            /// <param name="result"></param>
            void SetResult(object result);

            /// <summary>
            /// 设置异常
            /// </summary>
            /// <param name="ex"></param>
            void SetException(Exception ex);
        }


        /// <summary>
        /// 提供Task结果设置
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        private class TaskSetter<TResult> : ITaskSetter
        {
            private readonly TaskCompletionSource<TResult> taskSource;

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
            /// Task结果设置
            /// </summary>
            public TaskSetter()
            {
                this.taskSource = new TaskCompletionSource<TResult>();
            }

            /// <summary>
            /// 设置结果
            /// </summary>
            /// <param name="result"></param>
            public void SetResult(object result)
            {
                this.taskSource.TrySetResult((TResult)result);
            }

            /// <summary>
            /// 设置异常
            /// </summary>
            /// <param name="ex"></param>
            public void SetException(Exception ex)
            {
                this.taskSource.TrySetException(ex);
            }
        }
    }
}