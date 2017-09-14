using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示Http请求Header的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class HeaderAttribute : Attribute, IApiActionAttribute, IApiParameterAttribute
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
        Task IApiActionAttribute.BeforeRequestAsync(ApiActionContext context)
        {
            var header = context.RequestMessage.Headers;
            header.Remove(this.name);
            header.Add(this.name, this.value);
            return TaskExtend.CompletedTask;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        Task IApiParameterAttribute.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var header = context.RequestMessage.Headers;
            var valueString = parameter.Value == null ? null : parameter.Value.ToString();
            header.Remove(this.name);
            header.Add(this.name, valueString);
            return TaskExtend.CompletedTask;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.name;
        }
    }
}
