using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths;
using WebApiClientCore.Extensions.OAuths.Exceptions;
using WebApiClientCore.Extensions.OAuths.TokenProviders;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示同接口动态token应用特性
    /// 需要为多应用接口注册DynamicTokenProvider
    /// </summary>
    public class DynamicTokenAttribute : ApiFilterAttribute
    {
        /// <summary>
        /// 带参数模板的应用标识
        /// </summary>
        private readonly string _identifierTemplate;

        /// <summary>
        /// 参数格式化之后的应用标识
        /// </summary>
        private string? _identifier;

        /// <summary>
        /// 异步锁
        /// </summary>
        private readonly AsyncRoot asyncRoot = new AsyncRoot();

        /// <summary>
        /// 多接口token应用标识
        /// 支持 '{parameter}' 占位，取值于参数。
        /// </summary>
        /// <param name="identifierTemplate">标识名称</param>
        public DynamicTokenAttribute(string identifierTemplate)
        {
            _identifierTemplate = identifierTemplate;
        }

        /// <summary>
        /// 请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        public sealed override async Task OnRequestAsync(ApiRequestContext context)
        {
            if (await GetTokenProvider(context) is IDynamicTokenProvider provider)
            {
                var token = await provider.GetTokenAsync(_identifier).ConfigureAwait(false);
                UseTokenResult(context, token);
            }
            else
            {
                throw new TokenException($"动态Token提供者需要注册为 {typeof(IDynamicTokenProvider)}");
            }
        }

        /// <summary>
        /// 响应后
        /// </summary>
        /// <param name="context">上下文</param>
        public sealed override async Task OnResponseAsync(ApiResponseContext context)
        {
            if (IsUnauthorized(context) && await GetTokenProvider(context) is IDynamicTokenProvider provider)
            {
                await provider.ClearTokenAsync(_identifier).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// 动态参数格式化
        /// </summary>
        protected static string ParameterFormat(ApiRequestContext context, string template)
        {
            foreach (Match item in new Regex("\\{([^\\}]+)\\}").Matches(template).Cast<Match>())
            {
                if (context.TryGetArgument<string>(item.Groups[1].Value, out var t1))
                {
                    template = template.Replace(item.Groups[0].Value, t1);
                }
            }
            return template;
        }

        /// <summary>
        /// 获取token提供者
        /// </summary>
        /// <param name="context">上下文</param>
        protected virtual async Task<ITokenProvider> GetTokenProvider(ApiRequestContext context)
        {
            using (await asyncRoot.LockAsync().ConfigureAwait(false))
            {
                _identifier = ParameterFormat(context, _identifierTemplate);

                var factory = context.HttpContext.ServiceProvider.GetRequiredService<ITokenProviderFactory>();
                return factory.Create(context.ActionDescriptor.InterfaceType, TypeMatchMode.TypeOnly, _identifier);
            }
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
            return response?.StatusCode == HttpStatusCode.Unauthorized;
        }
    }
}
