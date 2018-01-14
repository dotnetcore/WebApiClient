using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;

namespace WebApiClient
{
    /// <summary>
    /// 表示请求Api的参数描述
    /// </summary>
    [DebuggerDisplay("{Name} = {Value}")]
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
        /// 获取关联的参数特性
        /// </summary>
        public IApiParameterAttribute[] Attributes { get; internal set; }

        /// <summary>
        /// 值转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value == null ? null : this.Value.ToString();
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
                Name = this.Name,
                ParameterType = this.ParameterType,
                Value = this.Value
            };
        }
    }
}
