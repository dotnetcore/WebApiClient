using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;

namespace WebApiClient.Contexts
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
        /// 获取参数类型是否为HttpContent类型
        /// </summary>
        public bool IsHttpContent { get; internal set; }

        /// <summary>
        /// 获取参数类型是否为简单类型
        /// </summary>
        public bool IsSimpleType { get; internal set; }

        /// <summary>
        /// 获取参数类型是否为可列举类型
        /// </summary>
        public bool IsEnumerable { get; internal set; }

        /// <summary>
        /// 获取参数类型是否为IDictionaryOf(string,object)
        /// </summary>
        public bool IsDictionaryOfObject { get; internal set; }

        /// <summary>
        /// 获取参数类型是否为IDictionaryOf(string,string)
        /// </summary>
        public bool IsDictionaryOfString { get; internal set; }

        /// <summary>
        /// 获取参数类型是否为IApiParameterable类型
        /// </summary>
        public bool IsApiParameterable { get; internal set; }

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
                IsApiParameterable = this.IsApiParameterable,
                IsHttpContent = this.IsHttpContent,
                IsSimpleType = this.IsSimpleType,
                IsEnumerable = this.IsEnumerable,
                IsDictionaryOfObject = this.IsDictionaryOfObject,
                IsDictionaryOfString = this.IsDictionaryOfString,
                Name = this.Name,
                ParameterType = this.ParameterType,
                Value = this.Value
            };
        }
    }
}
