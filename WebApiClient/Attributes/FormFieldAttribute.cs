using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
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
        private readonly string name;

        /// <summary>
        /// 字段的值
        /// </summary>
        private readonly string value;

        /// <summary>
        /// 表示参数值作为x-www-form-urlencoded的字段
        /// </summary>
        public FormFieldAttribute()
        {
        }

        /// <summary>
        /// 表示name和value写入x-www-form-urlencoded表单
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">字段的值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FormFieldAttribute(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            this.name = name;
            this.value = value == null ? null : value.ToString();
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

            var keyValue = new KeyValuePair<string, string>(this.name, this.value);
            var httpContent = await context.EnsureNoGet().RequestMessage.Content.MergeKeyValuesAsync(new[] { keyValue });
            context.RequestMessage.Content = httpContent;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        async Task IApiParameterAttribute.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var stringValue = parameter.Value == null ? null : parameter.Value.ToString();
            var keyValue = new KeyValuePair<string, string>(parameter.Name, stringValue);
            var httpContent = await context.EnsureNoGet().RequestMessage.Content.MergeKeyValuesAsync(new[] { keyValue });
            context.RequestMessage.Content = httpContent;
        }
    }
}
