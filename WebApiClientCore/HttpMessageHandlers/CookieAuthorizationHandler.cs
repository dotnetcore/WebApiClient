using System;
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
        protected sealed override async Task SetAuthorizationAsync(SetReason reason, HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (reason == SetReason.ForResend)
            {
                var handler = this.GetHttpClientHandler(this.InnerHandler);
                if (handler.UseCookies == false)
                {
                    throw new NotSupportedException(Resx.unsupported_NoUseCookies);
                }

                var response = await this.RefreshCookieAsync().ConfigureAwait(false);
                if (response.Headers.TryGetValues("Set-Cookie", out var cookies) == true)
                {
                    foreach (var cookie in cookies)
                    {
                        handler.CookieContainer.SetCookies(request.RequestUri, cookie);
                    }
                }
            }
        }

        /// <summary>
        /// 获取内部的HttpClientHandler
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        private HttpClientHandler GetHttpClientHandler(HttpMessageHandler handler)
        {
            if (handler is DelegatingHandler delegatingHandler)
            {
                return this.GetHttpClientHandler(delegatingHandler.InnerHandler);
            }

            if (handler is HttpClientHandler clientHandler)
            {
                return clientHandler;
            }

            throw new NotSupportedException(Resx.unsupported_HttpMessageHandler);
        }

        /// <summary>
        /// 登录并刷新Cookie
        /// </summary>
        /// <returns>返回登录响应消息</returns>
        protected abstract Task<HttpResponseMessage> RefreshCookieAsync();
    }
}
