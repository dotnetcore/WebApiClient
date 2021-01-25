using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示Http请求Header的特性
    /// </summary>
    [DebuggerDisplay("{name} = {value}")]
    public partial class HeaderAttribute : ApiActionAttribute
    {
        /// <summary>
        /// Header名称
        /// </summary>
        private readonly string name = string.Empty;

        /// <summary>
        /// Header值 
        /// </summary>
        private readonly string value = string.Empty;

        /// <summary>
        /// 将指定值设置到Header       
        /// </summary>
        /// <param name="name">header名称</param>
        /// <param name="value">header值</param>
        [AttributeCtorUsage(AttributeTargets.Interface | AttributeTargets.Method)]
        public HeaderAttribute(HttpRequestHeader name, string value)
            : this(name.ToHeaderName(), value)
        {
        }

        /// <summary>
        /// 将指定值设置到Header      
        /// </summary>
        /// <param name="name">header名称</param>
        /// <param name="value">header值</param>
        /// <exception cref="ArgumentNullException"></exception>
        [AttributeCtorUsage(AttributeTargets.Interface | AttributeTargets.Method)]
        public HeaderAttribute(string name, string value)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.value = value ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            context.HttpContext.RequestMessage.Headers.TryAddWithoutValidation(this.name, this.value);
            return Task.CompletedTask;
        }
    }
}
