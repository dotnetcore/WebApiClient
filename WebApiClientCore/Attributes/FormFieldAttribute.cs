using System;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数值作为x-www-form-urlencoded的字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class FormFieldAttribute : ApiActionAttribute, IApiParameterAttribute
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
        /// 表示参数值作为x-www-form-urlencoded的字段
        /// </summary>
        [AttributeCtorUsage(AttributeTargets.Parameter)]
        public FormFieldAttribute()
        {
        }

        /// <summary>
        /// 表示name和value写入x-www-form-urlencoded表单
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">字段的值</param>
        /// <exception cref="ArgumentNullException"></exception>
        [AttributeCtorUsage(AttributeTargets.Interface | AttributeTargets.Method)]
        public FormFieldAttribute(string name, object? value)
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
        public override async Task OnRequestAsync(ApiRequestContext context)
        {
            if (string.IsNullOrEmpty(this.name))
            {
                throw new ApiInvalidConfigException(Resx.required_NameAndValue);
            }

            await context.HttpContext.RequestMessage.AddFormFieldAsync(this.name, this.value).ConfigureAwait(false);
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public async Task OnRequestAsync(ApiParameterContext context)
        {
            await context.HttpContext.RequestMessage.AddFormFieldAsync(context.Parameter.Name, context.ParameterValue?.ToString()).ConfigureAwait(false);
        }
    }
}
