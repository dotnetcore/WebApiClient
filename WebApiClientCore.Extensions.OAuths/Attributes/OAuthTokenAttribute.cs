﻿using Microsoft.Extensions.DependencyInjection;
using System.Linq;
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
        /// 获取或设置token提供者的查找模式
        /// </summary>
        public TypeMatchMode TokenProviderSearchMode { get; set; } = TypeMatchMode.TypeOrBaseTypes;

        /// <summary>
        /// 请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public sealed override async Task OnRequestAsync(ApiRequestContext context)
        {
            var identifier = GetTokenKey(context);
            var token = await this.GetTokenProvider(context).GetTokenAsync(identifier).ConfigureAwait(false);
            this.UseTokenResult(context, token);
        }

        private static string GetTokenKey(ApiRequestContext context)
        {
            var parameter = context.ActionDescriptor.Parameters
                    .FirstOrDefault(p => p.ParameterType == typeof(OAuthTokenKeyAttribute));

            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            if (!context.TryGetArgument(parameter.Name, out string? identifier))
                throw new ArgumentNullException(nameof(parameter));

            return identifier ?? string.Empty;
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
            var factory = context.HttpContext.ServiceProvider.GetRequiredService<ITokenProviderFactory>();
            return factory.Create(context.ActionDescriptor.InterfaceType, this.TokenProviderSearchMode);
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
