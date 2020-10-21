using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClientCore.HttpMessageHandlers
{
    /// <summary>
    /// 表示cookie授权验证的抽象http消息处理程序
    /// </summary>
    public abstract class CookieAuthorizationHandler : AuthorizationHandler
    {
        /// <summary>
        /// 设置请求的授权信息
        /// </summary>
        /// <param name="reason">授权原因</param> 
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        protected sealed override async Task SetAuthorizationAsync(AuthorizationReason reason, HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (reason == AuthorizationReason.SetForResend)
            {
                await this.RefreshCookieAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 刷新CookieContainer的cookie
        /// 一般情况下，登录操作就得到最新的cookie，且自动覆盖到CookieContainer
        /// </summary>
        /// <returns></returns>
        protected abstract Task RefreshCookieAsync();
    }
}
