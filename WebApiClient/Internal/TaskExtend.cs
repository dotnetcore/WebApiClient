using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        /// 转换为TaskOf(TResult)类型
        /// </summary>
        /// <typeparam name="TResult">目标Task的Result的类型</typeparam>
        /// <param name="sourceTask">源Task</param>
        /// <returns></returns>
        public static Task<TResult> Cast<TResult>(this Task sourceTask)
        {
            return sourceTask.Cast<TResult>(sourceTask.GetType());
        }

        /// <summary>
        /// 转换为TaskOf(TResult)类型
        /// </summary>
        /// <typeparam name="TResult">目标Task的Result的类型</typeparam>
        /// <param name="sourceTask">源Task</param>
        /// <param name="sourceTaskType">源Task的类型</param>
        /// <returns></returns>
        public async static Task<TResult> Cast<TResult>(this Task sourceTask, Type sourceTaskType)
        {
            await sourceTask;
            return (TResult)TaskResult.GetResult(sourceTask, sourceTaskType);
        }

        /// <summary>
        /// 将objectTask转换为TaskOf(targetResultType)
        /// </summary>
        /// <param name="objectTask">源Task</param>
        /// <param name="targetResultType">目标Task的Result的类型</param>
        /// <param name="sourceTaskType">源Task的类型</param>    
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Task Cast(this Task<object> objectTask, Type targetResultType)
        {
            if (objectTask == null)
            {
                throw new ArgumentNullException();
            }

            var awaiter = objectTask.GetAwaiter();
            var taskSource = new TaskCompletionSource(targetResultType);

            awaiter.OnCompleted(() =>
            {
                try
                {
                    var result = awaiter.GetResult();
                    taskSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    taskSource.SetException(ex);
                }
            });
            return taskSource.Task;
        }

        /// <summary>
        /// 将TaskOf(object)转换为TaskOf(ResultType)
        /// </summary>
        /// <param name="sourceTask">源Task</param>
        /// <param name="targetResultType">目标Task的Result的类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Task Cast(this Task sourceTask, Type targetResultType)
        {
            return sourceTask.Cast(targetResultType, sourceTask.GetType());
        }

        /// <summary>
        /// 将sourceTask转换为TaskOf(targetResultType)
        /// </summary>
        /// <param name="sourceTask">源Task</param>
        /// <param name="targetResultType">目标Task的Result的类型</param>
        /// <param name="sourceTaskType">源Task的类型</param>    
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Task Cast(this Task sourceTask, Type targetResultType, Type sourceTaskType)
        {
            if (sourceTask == null)
            {
                throw new ArgumentNullException();
            }

            var awaiter = sourceTask.GetAwaiter();
            var taskSource = new TaskCompletionSource(targetResultType);

            awaiter.OnCompleted(() =>
            {
                try
                {
                    awaiter.GetResult();
                    var result = TaskResult.GetResult(sourceTask, sourceTaskType);
                    taskSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    taskSource.SetException(ex);
                }
            });
            return taskSource.Task;
        }


        /// <summary>
        /// 提供获取task的Result
        /// </summary>
        static class TaskResult
        {
            /// <summary>
            /// 安全字典
            /// </summary>
            private readonly static ConcurrentDictionary<Type, Func<Task, object>> resultDelegateCache;

            /// <summary>
            /// 获取task的Result
            /// </summary>
            static TaskResult()
            {
                resultDelegateCache = new ConcurrentDictionary<Type, Func<Task, object>>();
            }

            /// <summary>
            /// 获取task的Result属性值
            /// </summary>
            /// <param name="valueTask"></param>
            /// <param name="taskType">task的类型</param>
            /// <exception cref="ArgumentNullException"></exception>
            /// <returns></returns>
            public static object GetResult(Task valueTask, Type taskType)
            {
                if (valueTask == null)
                {
                    throw new ArgumentNullException("valueTask");
                }
                if (taskType == null)
                {
                    throw new ArgumentNullException("taskType");
                }

                var resultDelegate = resultDelegateCache.GetOrAdd(taskType, type => CreateResultDelegate(type));
                return resultDelegate.Invoke(valueTask);
            }

            /// <summary>
            /// 创建Task类型获取Result的委托
            /// </summary>
            /// <param name="taskType">Task实例的类型</param>
            /// <returns></returns>
            private static Func<Task, object> CreateResultDelegate(Type taskType)
            {
                if (taskType.IsGenericType == false || taskType.GetGenericTypeDefinition() != typeof(Task<>))
                {
                    return task => null;
                }

                // (Task task) => (object)(((Task<T>)task).Result)
                var arg = Expression.Parameter(typeof(Task));

                var castArg = Expression.Convert(arg, taskType);
                var resultFieldAccess = Expression.Property(castArg, "Result");
                var objectResult = Expression.Convert(resultFieldAccess, typeof(object));

                return Expression.Lambda<Func<Task, object>>(objectResult, arg).Compile();
            }
        }

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
            /// <exception cref="ArgumentNullException"></exception>
            public TaskCompletionSource(Type resultType)
            {
                if (resultType == null)
                {
                    throw new ArgumentNullException();
                }
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
}
