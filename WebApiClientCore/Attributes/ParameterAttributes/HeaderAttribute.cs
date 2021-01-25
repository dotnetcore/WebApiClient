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
        /// Header别名
        /// </summary>
        private readonly string? aliasName;

        /// <summary>
        /// 将参数值设置到Header        
        /// </summary>
        [AttributeCtorUsage(AttributeTargets.Parameter)]
        public HeaderAttribute()
        {
        }

        /// <summary>
        /// 将参数值设置到Header        
        /// </summary>
        /// <param name="aliasName">header别名</param>
        [AttributeCtorUsage(AttributeTargets.Parameter)]
        public HeaderAttribute(HttpRequestHeader aliasName)
            : this(aliasName.ToHeaderName())
        {
        }

        /// <summary>
        /// 将参数值设置到Header      
        /// </summary>
        /// <param name="aliasName">header别名</param>
        [AttributeCtorUsage(AttributeTargets.Parameter)]
        public HeaderAttribute(string aliasName)
        {
            this.aliasName = aliasName;
        }

        /// <summary>
        /// http请求之前
        /// 值从参数过来
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <returns></returns>
        public Task OnRequestAsync(ApiParameterContext context)
        {
            var headerName = this.aliasName;
            if (string.IsNullOrEmpty(headerName) == true)
            {
                headerName = context.ParameterName;
            }

            var headerValue = context.ParameterValue?.ToString();
            if (string.IsNullOrEmpty(headerValue) == false)
            {
                context.HttpContext.RequestMessage.Headers.TryAddWithoutValidation(headerName, headerValue);
            }
            return Task.CompletedTask;
        }
    }
}
