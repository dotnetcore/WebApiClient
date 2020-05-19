using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClientCore.OAuths;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示OAuth授权的token过滤器抽象
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public abstract class TokenFilterAttribute : ApiFilterAttribute
    {
        /// <summary>
        /// 请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public sealed override async Task OnRequestAsync(ApiRequestContext context)
        {
            var provider = this.GetTokenProvider(context);
            var token = await provider.GetTokenAsync(context.HttpContext).ConfigureAwait(false);
            this.UseTokenResult(context, token);
        }

        /// <summary>
        /// 获取token提供者
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected abstract TokenProvider GetTokenProvider(ApiRequestContext context);

        /// <summary>
        /// 应用token
        /// 默认为添加到请求头的Authorization
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="tokenResult">token结果</param>
        /// <returns></returns>
        protected virtual void UseTokenResult(ApiRequestContext context, TokenResult tokenResult)
        {
            var tokenType = tokenResult.Token_type ?? "Bearer";
            context.HttpContext.RequestMessage.Headers.Authorization = new AuthenticationHeaderValue(tokenType, tokenResult.Access_token);
        }
    }
}
