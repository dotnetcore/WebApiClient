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
            void OnEndConnect(object sender, SocketAsyncEventArgs e)
            {
                var setter = e.UserToken as TaskSetter<object>;
                if (e.SocketError == SocketError.Success)
                {
                    setter.SetResult(null);
                }
                else
                {
                    var ex = new SocketException((int)e.SocketError);
                    setter.SetException(ex);
                }
            }


            if (remoteEndPoint == null)
            {
                throw new ArgumentNullException(nameof(remoteEndPoint));
            }

            var taskSetter = new TaskSetter<object>(timeout);
            using (var args = new SocketAsyncEventArgs())
            {
                args.UserToken = taskSetter;
                args.RemoteEndPoint = remoteEndPoint;
                args.Completed += OnEndConnect;

                if (socket.ConnectAsync(args) == false)
                {
                    OnEndConnect(socket, args);
                }
                await taskSetter.Task.ConfigureAwait(false);
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
            void OnEndSend(object sender, SocketAsyncEventArgs e)
            {
                var setter = e.UserToken as TaskSetter<int>;
                if (e.SocketError == SocketError.Success)
                {
                    setter.SetResult(e.BytesTransferred);
                }
                else
                {
                    var ex = new SocketException((int)e.SocketError);
                    setter.SetException(ex);
                }
            }

            var taskSetter = new TaskSetter<int>(timeout);
            using (var args = new SocketAsyncEventArgs { UserToken = taskSetter })
            {
                args.SetBuffer(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
                args.Completed += OnEndSend;

                if (socket.SendAsync(args) == false)
                {
                    OnEndSend(socket, args);
                }
                return await taskSetter.Task.ConfigureAwait(false);
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
            void OnEndReceive(object sender, SocketAsyncEventArgs e)
            {
                var setter = e.UserToken as TaskSetter<int>;
                if (e.SocketError == SocketError.Success)
                {
                    setter.SetResult(e.BytesTransferred);
                }
                else
                {
                    var ex = new SocketException((int)e.SocketError);
                    setter.SetException(ex);
                }
            }

            var taskSetter = new TaskSetter<int>(timeout);
            using (var args = new SocketAsyncEventArgs { UserToken = taskSetter })
            {
                args.SetBuffer(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
                args.Completed += OnEndReceive;

                if (socket.ReceiveAsync(args) == false)
                {
                    OnEndReceive(socket, args);
                }
                return await taskSetter.Task.ConfigureAwait(false);
            }
        }


        /// <summary>
        /// 表示任务行为
        /// </summary>
        /// <typeparam name="TResult">任务结果类型</typeparam>
        private class TaskSetter<TResult>
        {
            /// <summary>
            /// 取消源
            /// </summary>
            private readonly CancellationTokenSource tokenSource;

            /// <summary>
            /// 任务源
            /// </summary>
            private readonly TaskCompletionSource<TResult> taskSource = new TaskCompletionSource<TResult>();

            /// <summary>
            /// 获取任务对象
            /// </summary>
            public Task<TResult> Task
            {
                get => this.taskSource.Task;
            }

            /// <summary>
            /// 任务行为
            /// </summary>
            public TaskSetter(TimeSpan? timeout)
            {
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
        }
    }
}
