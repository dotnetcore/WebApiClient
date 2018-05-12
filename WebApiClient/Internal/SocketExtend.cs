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
        /// <param name="timeout">连接的超时时间</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        public static async Task ConnectTaskAsync(this Socket socket, EndPoint remoteEndPoint, TimeSpan? timeout)
        {
            if (remoteEndPoint == null)
            {
                throw new ArgumentNullException(nameof(remoteEndPoint));
            }

            var token = new UserToken<object>
            {
                Socket = socket,
                TaskSetter = new TaskSetter<object>(timeout)
            };

            socket.BeginConnect(remoteEndPoint, OnEndConnect, token);
            await token.TaskSetter.Task;
        }

        /// <summary>
        /// 连接完成
        /// </summary>
        /// <param name="ar"></param>
        private static void OnEndConnect(IAsyncResult ar)
        {
            var token = ar.AsyncState as UserToken<object>;
            try
            {
                token.Socket.EndConnect(ar);
                token.TaskSetter.SetResult(null);
            }
            catch (Exception ex)
            {
                token.TaskSetter.SetException(ex);
            }
        }


        /// <summary>
        /// 异步发送
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="arraySegment">缓冲区</param>
        /// <param name="timeout">等待数据的超时时间</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        public static async Task<int> SendTaskAsync(this Socket socket, ArraySegment<byte> arraySegment, TimeSpan? timeout)
        {
            var token = new UserToken<int>
            {
                Socket = socket,
                TaskSetter = new TaskSetter<int>(timeout)
            };

            socket.BeginSend(arraySegment.Array, arraySegment.Offset, arraySegment.Count, SocketFlags.None, OnEndSend, token);
            return await token.TaskSetter.Task;
        }

        /// <summary>
        /// 发送完成
        /// </summary>
        /// <param name="ar"></param>
        private static void OnEndSend(IAsyncResult ar)
        {
            var token = ar.AsyncState as UserToken<int>;
            try
            {
                var length = token.Socket.EndSend(ar);
                token.TaskSetter.SetResult(length);
            }
            catch (Exception ex)
            {
                token.TaskSetter.SetException(ex);
            }
        }

        /// <summary>
        /// 异步接收
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="arraySegment">缓冲区</param>
        /// <param name="timeout">等待数据的超时时间</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        public static async Task<int> ReceiveTaskAsync(this Socket socket, ArraySegment<byte> arraySegment, TimeSpan? timeout)
        {
            var token = new UserToken<int>
            {
                Socket = socket,
                TaskSetter = new TaskSetter<int>(timeout)
            };
            socket.BeginReceive(arraySegment.Array, arraySegment.Offset, arraySegment.Count, SocketFlags.None, OnEndReceive, token);
            return await token.TaskSetter.Task;
        }

        /// <summary>
        /// 接收完成
        /// </summary>
        /// <param name="ar"></param>
        private static void OnEndReceive(IAsyncResult ar)
        {
            var token = ar.AsyncState as UserToken<int>;
            try
            {
                var length = token.Socket.EndReceive(ar);
                token.TaskSetter.SetResult(length);
            }
            catch (Exception ex)
            {
                token.TaskSetter.SetException(ex);
            }
        }

        /// <summary>
        /// 表示用户信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class UserToken<T>
        {
            /// <summary>
            /// socket
            /// </summary>
            public Socket Socket { get; set; }

            /// <summary>
            /// 任务
            /// </summary>
            public TaskSetter<T> TaskSetter { get; set; }
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
            public TaskSetter(TimeSpan? timeout)
            {
                this.taskSource = new TaskCompletionSource<TResult>();
                if (timeout.HasValue == true)
                {
                    this.tokenSource = new CancellationTokenSource();
                    this.tokenSource.Token.Register(() => this.SetException(new TimeoutException()));
                    this.tokenSource.CancelAfter(timeout.Value);
                }
            }

            /// <summary>
            /// 设置任务的行为结果
            /// </summary>     
            /// <param name="value">数据值</param>   
            /// <returns></returns>
            public bool SetResult(TResult value)
            {
                this.tokenSource?.Dispose();
                return this.taskSource.TrySetResult(value);
            }

            /// <summary>
            /// 设置设置为异常
            /// </summary>
            /// <param name="ex">异常</param>
            /// <returns></returns>
            public bool SetException(Exception ex)
            {
                this.tokenSource?.Dispose();
                return this.taskSource.TrySetException(ex);
            }

            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                this.tokenSource?.Dispose();
            }
        }
    }
}
