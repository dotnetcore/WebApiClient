using System;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数值作为x-www-form-urlencoded的字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public partial class FormFieldAttribute : IApiParameterAttribute
    {
        /// <summary>
        /// 表示参数值作为x-www-form-urlencoded的字段
        /// </summary>
        [AttributeCtorUsage(AttributeTargets.Parameter)]
        public FormFieldAttribute()
        {
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public async Task OnRequestAsync(ApiParameterContext context)
        {
            await context.HttpContext.RequestMessage.AddFormFieldAsync(context.ParameterName, context.ParameterValue?.ToString()).ConfigureAwait(false);
        }
    }
}
