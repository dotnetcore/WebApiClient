using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// Socket扩展
    /// </summary>
    static class SocketExtend
    {
        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="socket">socket</param>
        /// <param name="remoteEndPoint">远程终结点</param>
        /// <param name="timeout">超时时间</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        public static async Task ConnectTaskAsync(this Socket socket, EndPoint remoteEndPoint, TimeSpan timeout)
        {
            if (remoteEndPoint == null)
            {
                throw new ArgumentNullException(nameof(remoteEndPoint));
            }

            var taskSetter = new TaskSetter<object>(timeout);
            var asyncEventArg = new SocketAsyncEventArgs
            {
                UserToken = taskSetter,
                RemoteEndPoint = remoteEndPoint
            };

            asyncEventArg.Completed += ConnectCompleted;

            if (socket.ConnectAsync(asyncEventArg) == false)
            {
                ConnectCompleted(socket, asyncEventArg);
            }

            try
            {
                await taskSetter.Task;
            }
            finally
            {
                asyncEventArg.Dispose();
            }
        }

        /// <summary>
        /// 连接完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            var taskSetter = e.UserToken as TaskSetter<object>;
            taskSetter.SetResult(null);
        }

        /// <summary>
        /// 异步接收
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="arraySegment">缓冲区</param>
        /// <param name="timeout">超时时间</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        public static async Task<int> ReceiveTaskAsync(this Socket socket, ArraySegment<byte> arraySegment, TimeSpan timeout)
        {
            var taskSetter = new TaskSetter<int>(timeout);
            var asyncEventArg = new SocketAsyncEventArgs
            {
                UserToken = taskSetter
            };

            asyncEventArg.SetBuffer(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
            asyncEventArg.Completed += ReceiveCompleted;

            if (socket.ReceiveAsync(asyncEventArg) == false)
            {
                ReceiveCompleted(socket, asyncEventArg);
            }

            try
            {
                return await taskSetter.Task;
            }
            finally
            {
                asyncEventArg.Dispose();
            }
        }

        /// <summary>
        /// 接收完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            var taskSetter = e.UserToken as TaskSetter<int>;
            taskSetter.SetResult(e.BytesTransferred);
        }

        /// <summary>
        /// 表示任务行为
        /// </summary>
        /// <typeparam name="TResult">任务结果类型</typeparam>
        private class TaskSetter<TResult> : IDisposable
        {
            /// <summary>
            /// 任务源
            /// </summary>
            private readonly TaskCompletionSource<TResult> taskSource;

            /// <summary>
            /// 取消源
            /// </summary>
            private readonly CancellationTokenSource tokenSource;

            /// <summary>
            /// 获取任务的返回值类型
            /// </summary>
            public Type ValueType
            {
                get
                {
                    return typeof(TResult);
                }
            }

            /// <summary>
            /// 获取任务对象
            /// </summary>
            public Task<TResult> Task
            {
                get
                {
                    return this.taskSource.Task;
                }
            }

            /// <summary>
            /// 任务行为
            /// </summary>
            public TaskSetter(TimeSpan timeout)
            {
                this.taskSource = new TaskCompletionSource<TResult>();
                this.tokenSource = new CancellationTokenSource();
                this.tokenSource.Token.Register(() => this.SetException(new TimeoutException()));
                this.tokenSource.CancelAfter(timeout);
            }

            /// <summary>
            /// 设置任务的行为结果
            /// </summary>     
            /// <param name="value">数据值</param>   
            /// <returns></returns>
            public bool SetResult(TResult value)
            {
                this.tokenSource.Dispose();
                return this.taskSource.TrySetResult(value);
            }

            /// <summary>
            /// 设置设置为异常
            /// </summary>
            /// <param name="ex">异常</param>
            /// <returns></returns>
            public bool SetException(Exception ex)
            {
                this.tokenSource.Dispose();
                return this.taskSource.TrySetException(ex);
            }

            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                this.tokenSource.Dispose();
            }
        }
    }
}
