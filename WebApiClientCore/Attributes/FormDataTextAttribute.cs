using System;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数值作为multipart/form-data表单的一个文本项
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class FormDataTextAttribute : ApiActionAttribute, IApiParameterAttribute
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        private readonly string? name;

        /// <summary>
        /// 字段的值
        /// </summary>
        private readonly string? value;

        /// <summary>
        /// 表示参数值作为multipart/form-data表单的一个文本项
        /// </summary>
        [AttributeCtorUsage(AttributeTargets.Parameter)]
        public FormDataTextAttribute()
        {
        }

        /// <summary>
        /// 表示name和value写入multipart/form-data表单
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">字段的值</param>
        /// <exception cref="ArgumentNullException"></exception>
        [AttributeCtorUsage(AttributeTargets.Interface | AttributeTargets.Method)]
        public FormDataTextAttribute(string name, object? value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.name = name;
            this.value = value?.ToString();
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            if (string.IsNullOrEmpty(this.name))
            {
                throw new ApiInvalidConfigException(Resx.required_NameAndValue);
            }

            context.HttpContext.RequestMessage.AddFormDataText(this.name, this.value);
            return Task.CompletedTask;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public Task OnRequestAsync(ApiParameterContext context)
        {
            context.HttpContext.RequestMessage.AddFormDataText(context.Parameter.Name, context.ParameterValue?.ToString());
            return Task.CompletedTask;
        }
    }
}
