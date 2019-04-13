using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using WebApiClient.Attributes;
using WebApiClient.DataAnnotations;

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
        public string Name { get; protected set; }

        /// <summary>
        /// 获取关联的参数信息
        /// </summary>
        public ParameterInfo Member { get; protected set; }

        /// <summary>
        /// 获取参数索引
        /// </summary>
        public int Index { get; protected set; }

        /// <summary>
        /// 获取参数类型
        /// </summary>
        public Type ParameterType { get; protected set; }

        /// <summary>
        /// 获取参数值
        /// </summary>
        public object Value { get; protected set; }

        /// <summary>
        /// 获取关联的参数特性
        /// </summary>
        public IReadOnlyList<IApiParameterAttribute> Attributes { get; protected set; }

        /// <summary>
        /// 获取关联的ValidationAttribute特性
        /// </summary>
        public IReadOnlyList<ValidationAttribute> ValidationAttributes { get; protected set; }

        /// <summary>
        /// 请求Api的参数描述
        /// </summary>
        protected ApiParameterDescriptor()
        {
        }

        /// <summary>
        /// 请求Api的参数描述
        /// </summary>
        /// <param name="parameter">参数信息</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiParameterDescriptor(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var parameterType = parameter.ParameterType;
            var parameterAlias = parameter.GetCustomAttribute<AliasAsAttribute>();
            var parameterName = parameterAlias == null ? parameter.Name : parameterAlias.Name;

            var defined = parameter.GetAttributes<IApiParameterAttribute>(true);
            var attributes = this
                .GetAttributes(parameterType, defined)
                .ToReadOnlyList();

            var validationAttributes = parameter
                .GetCustomAttributes<ValidationAttribute>(true)
                .ToReadOnlyList();

            this.Value = null;
            this.Member = parameter;
            this.Name = parameterName;
            this.Index = parameter.Position;
            this.Attributes = attributes;
            this.ParameterType = parameterType;
            this.ValidationAttributes = validationAttributes;
        }

        /// <summary>
        /// 值转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value?.ToString();
        }

        /// <summary>
        /// 克隆新设置新的值
        /// </summary>
        /// <param name="value">新的参数值</param>
        /// <returns></returns>
        public virtual ApiParameterDescriptor Clone(object value)
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

        /// <summary>
        /// 返回参数特性
        /// </summary>
        /// <param name="parameterType">参数类型</param>
        /// <param name="defined">参数上声明的特性</param>
        /// <returns></returns>
        protected virtual IEnumerable<IApiParameterAttribute> GetAttributes(Type parameterType, IEnumerable<IApiParameterAttribute> defined)
        {
            var attributes = new ParameterAttributeCollection(defined);
            var isHttpContent = parameterType.IsInheritFrom<HttpContent>();
            var isApiParameterable = parameterType.IsInheritFrom<IApiParameterable>() || parameterType.IsInheritFrom<IEnumerable<IApiParameterable>>();

            if (isApiParameterable == true)
            {
                attributes.Add(new ParameterableAttribute());
            }
            else if (isHttpContent == true)
            {
                attributes.AddIfNotExists(new HttpContentAttribute());
            }
            else if (parameterType == typeof(CancellationToken))
            {
                attributes.Add(new CancellationTokenAttribute());
            }
            else if (attributes.Count == 0)
            {
                attributes.Add(new PathQueryAttribute());
            }
            return attributes;
        }

        /// <summary>
        /// 表示参数特性集合
        /// </summary>
        private class ParameterAttributeCollection : IEnumerable<IApiParameterAttribute>
        {
            /// <summary>
            /// 特性列表
            /// </summary>
            private readonly List<IApiParameterAttribute> attributeList = new List<IApiParameterAttribute>();

            /// <summary>
            /// 获取元素数量
            /// </summary>
            public int Count
            {
                get => this.attributeList.Count;
            }

            /// <summary>
            /// 参数特性集合
            /// </summary>
            /// <param name="defined">声明的特性</param>
            public ParameterAttributeCollection(IEnumerable<IApiParameterAttribute> defined)
            {
                this.attributeList.AddRange(defined);
            }

            /// <summary>
            /// 添加新特性
            /// </summary>
            /// <param name="attribute">新特性</param>
            public void Add(IApiParameterAttribute attribute)
            {
                this.attributeList.Add(attribute);
            }

            /// <summary>
            /// 添加新特性
            /// </summary>
            /// <param name="attribute">新特性</param>
            /// <returns></returns>
            public bool AddIfNotExists(IApiParameterAttribute attribute)
            {
                var type = attribute.GetType();
                if (this.attributeList.Any(item => item.GetType() == type) == true)
                {
                    return false;
                }

                this.attributeList.Add(attribute);
                return true;
            }

            /// <summary>
            /// 返回迭代器
            /// </summary>
            /// <returns></returns>
            public IEnumerator<IApiParameterAttribute> GetEnumerator()
            {
                return this.attributeList.GetEnumerator();
            }

            /// <summary>
            /// 返回迭代器
            /// </summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
