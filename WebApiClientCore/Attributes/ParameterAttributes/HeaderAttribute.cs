using System;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示Http请求Header的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public partial class HeaderAttribute : IApiParameterAttribute
    {
        /// <summary>
        /// 将参数值设置到Header        
        /// </summary>
        /// <param name="name">header名称</param>
        [AttributeCtorUsage(AttributeTargets.Parameter)]
        public HeaderAttribute(HttpRequestHeader name)
            : this(RequestHeader.GetName(name))
        {
        }

        /// <summary>
        /// 将参数值设置到Header      
        /// </summary>
        /// <param name="name">header名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        [AttributeCtorUsage(AttributeTargets.Parameter)]
        public HeaderAttribute(string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// http请求之前
        /// 值从参数过来
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <returns></returns>
        public Task OnRequestAsync(ApiParameterContext context)
        {
            var headerValue = context.ParameterValue?.ToString();
            if (string.IsNullOrEmpty(headerValue) == false)
            {
                context.HttpContext.RequestMessage.Headers.TryAddWithoutValidation(this.name, headerValue);
            }
            return Task.CompletedTask;
        }
    }
}
