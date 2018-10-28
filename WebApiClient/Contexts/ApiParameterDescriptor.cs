using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace WebApiClient.Contexts
{
    /// <summary>
    /// 表示请求Api的参数描述
    /// </summary>
    [DebuggerDisplay("{Name} = {Value}")]
    public class ApiParameterDescriptor
    {
        /// <summary>
        /// 获取参数名称
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 获取关联的参数信息
        /// </summary>
        public ParameterInfo Member { get; internal set; }

        /// <summary>
        /// 获取参数索引
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        /// 获取参数类型
        /// </summary>
        public Type ParameterType { get; internal set; }

        /// <summary>
        /// 获取参数值
        /// </summary>
        public object Value { get; internal set; }

        /// <summary>
        /// 获取关联的参数特性
        /// </summary>
        public IApiParameterAttribute[] Attributes { get; internal set; }

        /// <summary>
        /// 获取关联的ValidationAttribute特性
        /// </summary>
        public ValidationAttribute[] ValidationAttributes { get; internal set; }

        /// <summary>
        /// 值转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value?.ToString();
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public ApiParameterDescriptor Clone()
        {
            return this.Clone(this.Value);
        }

        /// <summary>
        /// 克隆新设置新的值
        /// </summary>
        /// <param name="value">新的参数值</param>
        /// <returns></returns>
        public ApiParameterDescriptor Clone(object value)
        {
            return new ApiParameterDescriptor
            {
                Name = this.Name,
                Index = this.Index,
                Value = value,
                Member = this.Member,
                Attributes = this.Attributes,
                ParameterType = this.ParameterType,
                ValidationAttributes = this.ValidationAttributes
            };
        }
    }
}
