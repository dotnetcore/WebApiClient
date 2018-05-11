using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示Http请求Header的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    [DebuggerDisplay("{name} = {value}")]
    public class HeaderAttribute : ApiActionAttribute, IApiParameterAttribute
    {
        /// <summary>
        /// Header名称
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Header值 
        /// </summary>
        private readonly string value;

        /// <summary>
        /// 获取是对cookie的Value进行Url utf-8编码
        /// 默认为true
        /// </summary>
        public bool EncodeCookie { get; set; }

        /// <summary>
        /// 将参数值设置到Header        
        /// </summary>
        /// <param name="name">header名称</param>
        public HeaderAttribute(HttpRequestHeader name)
            : this(RequestHeader.GetName(name), null)
        {
        }

        /// <summary>
        /// 将参数值设置到Header      
        /// </summary>
        /// <param name="name">header名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HeaderAttribute(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// 将指定值设置到Header       
        /// </summary>
        /// <param name="name">header名称</param>
        /// <param name="value">header值</param>
        public HeaderAttribute(HttpRequestHeader name, string value)
            : this(RequestHeader.GetName(name), value)
        {
        }

        /// <summary>
        /// 将指定值设置到Header      
        /// </summary>
        /// <param name="name">header名称</param>
        /// <param name="value">header值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HeaderAttribute(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            this.name = name;
            this.value = value;
            this.EncodeCookie = true;
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            this.SetHeaderValue(context, this.value);
            return ApiTask.CompletedTask;
        }

        /// <summary>
        /// http请求之前
        /// 值从参数过来
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        Task IApiParameterAttribute.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            this.SetHeaderValue(context, parameter.ToString());
            return ApiTask.CompletedTask;
        }

        /// <summary>
        /// 设置请求头
        /// </summary>
        /// <param name="context"></param>
        /// <param name="headerValue"></param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <returns></returns>
        private bool SetHeaderValue(ApiActionContext context, string headerValue)
        {
            if (string.Equals(this.name, "Cookie", StringComparison.OrdinalIgnoreCase))
            {
                return this.SetCookie(context, headerValue);
            }

            var headers = context.RequestMessage.Headers;
            headers.Remove(this.name);
            if (headerValue != null)
            {
                return headers.TryAddWithoutValidation(this.name, headerValue);
            }
            return false;
        }

        /// <summary>
        /// 设置Cookie值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="cookieValues">cookie值</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <exception cref="CookieException"></exception>
        /// <returns></returns>
        private bool SetCookie(ApiActionContext context, string cookieValues)
        {
            if (context.HttpApiConfig.HttpClient.Handler.UseCookies == false)
            {
                return this.EncodeCookie ?
                     context.RequestMessage.SetCookie(cookieValues) :
                     context.RequestMessage.SetRawCookie(cookieValues);
            }

            var domain = context.RequestMessage.RequestUri;
            if (domain == null)
            {
                throw new HttpApiConfigException("未配置HttpHost，无法应用Cookie");
            }

            return this.EncodeCookie ?
                context.HttpApiConfig.HttpClient.SetCookie(domain, cookieValues) :
                context.HttpApiConfig.HttpClient.SetRawCookie(domain, cookieValues);
        }
    }
}
