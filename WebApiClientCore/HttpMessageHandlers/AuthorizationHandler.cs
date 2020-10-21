using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClientCore.HttpMessageHandlers
{
    /// <summary>
    /// 表示授权应用的抽象http消息处理程序
    /// </summary>
    public abstract class AuthorizationHandler : DelegatingHandler
    {
        /// <summary>
        /// 检测响应是否未授权
        /// 未授权则重试请求一次
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await this.SetAuthorizationAsync(AuthorizationReason.SetForSend, request, cancellationToken).ConfigureAwait(false);
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (await this.IsUnauthorizedAsync(response).ConfigureAwait(false) == true)
            {
                await this.SetAuthorizationAsync(AuthorizationReason.SetForResend, request, cancellationToken).ConfigureAwait(false);
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            return response;
        }

        /// <summary>
        /// 设置请求的授权信息
        /// </summary>
        /// <param name="reason">授权原因</param> 
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        protected abstract Task SetAuthorizationAsync(AuthorizationReason reason, HttpRequestMessage request, CancellationToken cancellationToken);

        /// <summary>
        /// 返回响应是否为未授权状态
        /// 反回true则强制清除token以支持下次获取到新的token
        /// </summary>
        /// <param name="response"></param> 
        protected virtual Task<bool> IsUnauthorizedAsync(HttpResponseMessage? response)
        {
            var state = response != null && response.StatusCode == HttpStatusCode.Unauthorized;
            return Task.FromResult(state);
        }
    }
}
