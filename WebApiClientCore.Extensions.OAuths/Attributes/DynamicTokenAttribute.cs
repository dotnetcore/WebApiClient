using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths;
using WebApiClientCore.Extensions.OAuths.Exceptions;
using WebApiClientCore.Extensions.OAuths.TokenProviders;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示多接口token应用特性
    /// 需要为多应用接口注册DynamicTokenProvider
    /// </summary>
    public class DynamicTokenAttribute : OAuthTokenAttribute
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
        public override async Task OnRequestAsync(ApiRequestContext context)
        {
            if (GetTokenProvider(context) is IDynamicTokenProvider provider)
            {
                _identifier = context.ParameterFormat(_identifierTemplate);
                var token = await provider.GetTokenAsync(_identifier).ConfigureAwait(false);
                UseTokenResult(context, token);
            }
            else
            {
                throw new TokenException($"多应用Token提供者需要注册为 {typeof(IDynamicTokenProvider)}");
            }
        }

        /// <summary>
        /// 响应后
        /// </summary>
        /// <param name="context">上下文</param>
        public override async Task OnResponseAsync(ApiResponseContext context)
        {
            if (IsUnauthorized(context) && GetTokenProvider(context) is IDynamicTokenProvider provider)
            {
                await provider.ClearTokenAsync(_identifier).ConfigureAwait(false);
            }
        }
    }
}
