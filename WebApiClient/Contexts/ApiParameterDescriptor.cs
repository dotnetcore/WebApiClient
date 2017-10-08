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
        /// 转换为字符串
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


        /// <summary>
        /// 格式化参数为文本
        /// </summary>
        /// <param name="stringFormater">广本格式化工具</param>
        /// <param name="encoding">格式化编码</param>        
        /// <returns></returns>
        public string FormatAsString(IStringFormatter stringFormater, Encoding encoding)
        {
            if (this.Value == null)
            {
                return null;
            }

            if (this.ParameterType == typeof(string))
            {
                return this.Value.ToString();
            }

            return stringFormater.Serialize(this.Value, encoding);
        }


        #region key-value format

        /// <summary>
        /// 格式化参数为键值对
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> FormatAsKeyValues()
        {
            if (this.IsSimpleType == true)
            {
                var kv = this.FormatAsSimple(this.Name, this.Value);
                return new[] { kv };
            }

            if (this.IsDictionaryOfString == true)
            {
                return this.FormatAsDictionary<string>();
            }

            if (this.IsDictionaryOfObject == true)
            {
                return this.FormatAsDictionary<object>();
            }

            if (this.IsEnumerable == true)
            {
                return this.ForamtAsEnumerable();
            }

            return this.FormatAsComplex();
        }

        /// <summary>
        /// 数组为键值对
        /// </summary>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> ForamtAsEnumerable()
        {
            var array = this.Value as IEnumerable;
            if (array == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return from item in array.Cast<object>()
                   select this.FormatAsSimple(this.Name, item);
        }

        /// <summary>
        /// 复杂类型为键值对
        /// </summary>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> FormatAsComplex()
        {
            var instance = this.Value;
            if (instance == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return
                from p in Property.GetProperties(this.ParameterType)
                let value = p.GetValue(instance)
                select this.FormatAsSimple(p.Name, value);
        }

        /// <summary>
        /// 字典转换为键值对
        /// </summary>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> FormatAsDictionary<TValue>()
        {
            var dic = this.Value as IDictionary<string, TValue>;
            if (dic == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return from kv in dic select this.FormatAsSimple(kv.Key, kv.Value);
        }

        /// <summary>
        /// 简单类型为键值对
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private KeyValuePair<string, string> FormatAsSimple(string name, object value)
        {
            var valueString = value == null ? null : value.ToString();
            return new KeyValuePair<string, string>(name, valueString);
        }

        #endregion
    }
}
