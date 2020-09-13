using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading;
using WebApiClientCore;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///  表示HttpApiOptions变化令牌源
    /// </summary>
    /// <typeparam name="THttpApi">接口类型</typeparam>
    class HttpApiOptionsChangeTokenSource<THttpApi> : IOptionsChangeTokenSource<HttpApiOptions>, IHttpApiOptionsConfigureTrigger
    {
        /// <summary>
        /// 变化令牌
        /// </summary>
        private ChangeToken changeToken = new ChangeToken();

        /// <summary>
        /// 获取名称
        /// </summary>
        public string Name { get; } = HttpApi.GetName<THttpApi>();

        /// <summary>
        /// 获取变化令牌对象
        /// </summary>
        /// <returns></returns>
        public IChangeToken GetChangeToken()
        {
            return this.changeToken;
        }

        /// <summary>
        /// 触发HttpApiOptions的Action配置变化
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        public void Raise(Type httpApiType)
        {
            if (httpApiType == typeof(THttpApi))
            {
                Interlocked.Exchange(ref this.changeToken, new ChangeToken()).RaiseChange();
            }
        }

        /// <summary>
        /// 变化令牌
        /// </summary>
        private class ChangeToken : IChangeToken
        {
            /// <summary>
            /// 取消令牌源
            /// </summary>
            private readonly CancellationTokenSource cts = new CancellationTokenSource();

            /// <summary>
            /// 变化时是否触发回调
            /// </summary>
            public bool ActiveChangeCallbacks => true;

            /// <summary>
            /// 获取是否已变化
            /// </summary>
            public bool HasChanged => this.cts.IsCancellationRequested;

            /// <summary>
            /// 注册变化回调
            /// </summary>
            /// <param name="callback">回调</param>
            /// <param name="state">状态数据</param>
            /// <returns>用于反注册的取消者</returns>
            public IDisposable RegisterChangeCallback(Action<object> callback, object state)
            {
                return this.cts.Token.Register(callback, state);
            }

            /// <summary>
            /// 触发变化，调用所有注册的回调
            /// </summary>
            public void RaiseChange()
            {
                this.cts.Cancel();
            }
        }
    }
}
