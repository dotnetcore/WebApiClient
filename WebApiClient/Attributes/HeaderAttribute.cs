using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
        /// 名称
        /// </summary>
        private readonly string name;

        /// <summary>
        /// 值 
        /// </summary>
        private readonly string value;


        /// <summary>
        /// 将参数值设置到Header        
        /// </summary>
        /// <param name="name">header名称</param>
        public HeaderAttribute(HttpRequestHeader name)
            : this(name.ToString(), null)
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
            : this(name.ToString(), value)
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
                throw new ArgumentNullException("name");
            }
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            this.SetHeaderValue(context, this.value);
            return TaskExtend.CompletedTask;
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
            return TaskExtend.CompletedTask;
        }

        /// <summary>
        /// 设置请求头
        /// </summary>
        /// <param name="context"></param>
        /// <param name="headerValue"></param>
        private void SetHeaderValue(ApiActionContext context, string headerValue)
        {
            if (string.Equals(this.name, "Cookie", StringComparison.OrdinalIgnoreCase))
            {
                this.SetCookie(context, headerValue);
            }
            else
            {
                var header = context.RequestMessage.Headers;
                header.Remove(this.name);
                if (headerValue != null)
                {
                    header.TryAddWithoutValidation(this.name, headerValue);
                }
            }
        }

        /// <summary>
        /// 设置Cookie值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="cookieValues">cookie值</param>
        private void SetCookie(ApiActionContext context, string cookieValues)
        {
            var useContainer = context.HttpApiConfig.HttpClientHandler.UseCookies;
            if (useContainer == true)
            {
                this.SetCookieToContainer(context, cookieValues);
            }
            else
            {
                this.SetCookieToHeader(context, cookieValues);
            }
        }

        /// <summary>
        /// 设置Cookie值到请求头
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="cookieValues">cookie值</param>
        private void SetCookieToHeader(ApiActionContext context, string cookieValues)
        {
            var header = context.RequestMessage.Headers;
            const string cookieName = "Cookie";
            header.Remove(cookieName);

            var cookieItems = this
               .GetCookies(cookieValues)
               .Select(item => string.Format("{0}={1}", item.Name, item.Value));

            var cookieHeader = string.Join("; ", cookieItems);
            if (string.IsNullOrEmpty(cookieHeader) == false)
            {
                header.TryAddWithoutValidation(cookieName, cookieHeader);
            }
        }

        /// <summary>
        /// 设置Cookie值到容器
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="cookieValues">cookie值</param>
        private void SetCookieToContainer(ApiActionContext context, string cookieValues)
        {
            var baseUrl = context.RequestMessage.RequestUri;
            var container = context.HttpApiConfig.HttpClientHandler.CookieContainer;
            foreach (var cookie in this.GetCookies(cookieValues))
            {
                container.Add(baseUrl, cookie);
            }
        }

        /// <summary>
        /// 获取Cookie项
        /// </summary>
        /// <param name="cookieValues"></param>
        /// <returns></returns>
        private IEnumerable<Cookie> GetCookies(string cookieValues)
        {
            if (cookieValues == null)
            {
                return Enumerable.Empty<Cookie>();
            }

            return from item in cookieValues.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                   let kv = item.Split('=')
                   let name = kv.FirstOrDefault().Trim()
                   let value = kv.Length > 1 ? kv.LastOrDefault() : string.Empty
                   let encode = HttpUtility.UrlEncode(value, Encoding.UTF8)
                   select new Cookie(name, encode);
        }
    }
}
