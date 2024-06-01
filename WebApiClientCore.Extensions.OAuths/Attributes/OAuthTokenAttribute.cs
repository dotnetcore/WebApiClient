using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示token应用特性
    /// 需要为接口或接口的基础接口注册TokenProvider
    /// </summary> 
    /// <remarks>
    /// <para>• Client模式：services.AddClientCredentialsTokenProvider</para>
    /// <para>• Password模式：services.AddPasswordCredentialsTokenProvider</para>
    /// </remarks>
    public class OAuthTokenAttribute : ApiFilterAttribute
    {
        /// <summary>
        /// 获取指定TokenProvider别名的方法参数名
        /// </summary>
        public string? AliasParameterName { get; }

        /// <summary>
        /// 获取或设置token提供者的查找模式
        /// </summary>
        public TypeMatchMode TokenProviderSearchMode { get; set; } = TypeMatchMode.TypeOrBaseTypes;

        /// <summary>
        /// token应用特性
        /// </summary>
        public OAuthTokenAttribute()
        {
        }

        /// <summary>
        /// token应用特性
        /// </summary>
        /// <param name="aliasParameterName">指定TokenProvider别名的方法参数名</param>
        public OAuthTokenAttribute(string? aliasParameterName)
        {
            this.AliasParameterName = aliasParameterName;
        }

        /// <summary>
        /// 请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public sealed override async Task OnRequestAsync(ApiRequestContext context)
        {
            var token = await this.GetTokenProvider(context).GetTokenAsync().ConfigureAwait(false);
            this.UseTokenResult(context, token);
        }

        /// <summary>
        /// 响应后
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public sealed override Task OnResponseAsync(ApiResponseContext context)
        {
            if (this.IsUnauthorized(context) == true)
            {
                this.GetTokenProvider(context).ClearToken();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取token提供者
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected virtual ITokenProvider GetTokenProvider(ApiRequestContext context)
        {
            var alias = string.Empty;
            if (string.IsNullOrEmpty(this.AliasParameterName) == false)
            {
                if (context.TryGetArgument<string>(this.AliasParameterName, StringComparer.OrdinalIgnoreCase, out var aliasValue))
                {
                    alias = aliasValue;
                }
                else
                {
                    throw new InvalidOperationException($"未提供有效的参数值: {this.AliasParameterName}");
                }
            }

            var factory = context.HttpContext.ServiceProvider.GetRequiredService<ITokenProviderFactory>();
            return factory.Create(context.ActionDescriptor.InterfaceType, this.TokenProviderSearchMode, alias);
        }

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

        /// <summary>
        /// 返回响应是否为未授权状态
        /// 反回true则强制清除token以支持下次获取到新的token
        /// </summary>
        /// <param name="context"></param>
        protected virtual bool IsUnauthorized(ApiResponseContext context)
        {
            var response = context.HttpContext.ResponseMessage;
            return response != null && response.StatusCode == HttpStatusCode.Unauthorized;
        }
    }
}
