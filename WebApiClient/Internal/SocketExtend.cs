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

            var token = new TaskSetter<object>(timeout);
            var e = new SocketAsyncEventArgs
            {
                RemoteEndPoint = remoteEndPoint,
                UserToken = token
            };

            using (e)
            {
                e.Completed += OnEndConnect;
                if (socket.ConnectAsync(e) == false)
                {
                    OnEndConnect(socket, e);
                }
                await token.Task.ConfigureAwait(false);
            }
        }



        /// <summary>
        /// 连接完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnEndConnect(object sender, SocketAsyncEventArgs e)
        {
            var token = e.UserToken as TaskSetter<object>;
            if (e.SocketError == SocketError.Success)
            {
                token.SetResult(null);
            }
            else
            {
                var ex = new SocketException((int)e.SocketError);
                token.SetException(ex);
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
            var token = new TaskSetter<int>(timeout);
            var e = new SocketAsyncEventArgs
            {
                UserToken = token
            };

            using (e)
            {
                e.SetBuffer(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
                e.Completed += OnEndSend;

                if (socket.SendAsync(e) == false)
                {
                    OnEndSend(socket, e);
                }
                return await token.Task.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 发送完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnEndSend(object sender, SocketAsyncEventArgs e)
        {
            var token = e.UserToken as TaskSetter<int>;
            if (e.SocketError == SocketError.Success)
            {
                token.SetResult(e.BytesTransferred);
            }
            else
            {
                var ex = new SocketException((int)e.SocketError);
                token.SetException(ex);
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
            var token = new TaskSetter<int>(timeout);
            var e = new SocketAsyncEventArgs
            {
                UserToken = token
            };

            using (e)
            {
                e.SetBuffer(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
                e.Completed += OnEndSend;

                if (socket.ReceiveAsync(e) == false)
                {
                    OnEndReceive(socket, e);
                }
                return await token.Task.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 接收完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnEndReceive(object sender, SocketAsyncEventArgs e)
        {
            var token = e.UserToken as TaskSetter<int>;
            if (e.SocketError == SocketError.Success)
            {
                token.SetResult(e.BytesTransferred);
            }
            else
            {
                var ex = new SocketException((int)e.SocketError);
                token.SetException(ex);
            }
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
