using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示参数值作为multipart/form-data表单的一个文本项
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class MulitpartTextAttribute : ApiActionAttribute, IApiParameterAttribute
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        private readonly string name;

        /// <summary>
        /// 字段的值
        /// </summary>
        private readonly string value;

        /// <summary>
        /// 获取或设置当值为null是否忽略提交
        /// 默认为false
        /// </summary>
        public bool IgnoreWhenNull { get; set; }

        /// <summary>
        /// 表示参数值作为multipart/form-data表单的一个文本项
        /// </summary>
        public MulitpartTextAttribute()
        {
        }

        /// <summary>
        /// 表示name和value写入multipart/form-data表单
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">字段的值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MulitpartTextAttribute(string name, object value)
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
        public override async Task BeforeRequestAsync(ApiActionContext context)
        {
            if (string.IsNullOrEmpty(this.name))
            {
                throw new NotSupportedException("请传入name和value参数：" + this.GetType().Name);
            }

            if (this.WillIgnore(this.value) == false)
            {
                context.RequestMessage.AddMulitpartText(this.name, this.value);
                await ApiTask.CompletedTask;
            }
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        async Task IApiParameterAttribute.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (this.WillIgnore(parameter.Value) == false)
            {
                context.RequestMessage.AddMulitpartText(parameter.Name, parameter.ToString());
                await ApiTask.CompletedTask;
            }
        }

        /// <summary>
        /// 返回是否应该忽略提交 
        /// </summary>
        /// <param name="val">值</param>
        /// <returns></returns>
        private bool WillIgnore(object val)
        {
            return this.IgnoreWhenNull == true && val == null;
        }
    }
}
