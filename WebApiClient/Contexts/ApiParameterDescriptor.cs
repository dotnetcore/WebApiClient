using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;

namespace WebApiClient.Contexts
{
    /// <summary>
    /// 表示请求Api的参数描述
    /// </summary>
    public class ApiParameterDescriptor : ICloneable
    {
        /// <summary>
        /// 获取参数名称
        /// </summary>
        public string Name { get; internal set; }

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
        /// 获取参数类型是否为简单类型
        /// </summary>
        public bool IsSimpleType { get; internal set; }

        /// <summary>
        /// 获取参数类型是否为可列举类型
        /// </summary>
        public bool IsEnumerable { get; internal set; }

        /// <summary>
        /// 获取关联的参数特性
        /// </summary>
        public IApiParameterAttribute[] Attributes { get; internal set; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} = {1}", this.Name, this.Value);
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new ApiParameterDescriptor
            {
                Attributes = this.Attributes,
                Index = this.Index,
                IsSimpleType = this.IsSimpleType,
                IsEnumerable = this.IsEnumerable,
                Name = this.Name,
                ParameterType = this.ParameterType,
                Value = this.Value
            };
        }
    }
}
