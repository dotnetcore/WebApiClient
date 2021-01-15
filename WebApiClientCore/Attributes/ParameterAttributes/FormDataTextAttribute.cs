using System;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数值作为multipart/form-data表单的一个文本项
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public partial class FormDataTextAttribute : IApiParameterAttribute
    {
        /// <summary>
        /// 表示参数值作为multipart/form-data表单的一个文本项
        /// </summary>
        [AttributeCtorUsage(AttributeTargets.Parameter)]
        public FormDataTextAttribute()
        {
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public Task OnRequestAsync(ApiParameterContext context)
        {
            context.HttpContext.RequestMessage.AddFormDataText(context.ParameterName, context.ParameterValue?.ToString());
            return Task.CompletedTask;
        }
    }
}
