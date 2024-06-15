using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using WebApiClientCore.Attributes;
using WebApiClientCore.Implementations.TypeAttributes;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示请求Api的参数描述
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public class DefaultApiParameterDescriptor : ApiParameterDescriptor
    {
        /// <summary>
        /// 缺省参数特性时的默认特性
        /// </summary>
        private static readonly IApiParameterAttribute defaultAttribute = new PathQueryAttribute();

        /// <summary>
        /// 获取参数名称
        /// </summary>
        public override string Name { get; protected set; }

        /// <summary>
        /// 获取关联的参数信息
        /// </summary>
        public override ParameterInfo Member { get; protected set; }

        /// <summary>
        /// 获取参数索引
        /// </summary>
        public override int Index { get; protected set; }

        /// <summary>
        /// 获取参数类型
        /// </summary>
        public override Type ParameterType { get; protected set; }

        /// <summary>
        /// 获取关联的参数特性
        /// </summary>
        public override IReadOnlyList<IApiParameterAttribute> Attributes { get; protected set; }

        /// <summary>
        /// 获取关联的ValidationAttribute特性
        /// </summary>
        public override IReadOnlyList<ValidationAttribute> ValidationAttributes { get; protected set; }

        /// <summary>
        /// 请求Api的参数描述
        /// </summary>
        /// <param name="parameter">参数信息</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DefaultApiParameterDescriptor(ParameterInfo parameter)
            : this(parameter, defaultAttribute)
        {
        }

        /// <summary>
        /// 请求Api的参数描述
        /// </summary>
        /// <param name="parameter">参数信息</param>
        /// <param name="defaultAttribute">缺省特性时使用的默认特性</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DefaultApiParameterDescriptor(ParameterInfo parameter, IApiParameterAttribute defaultAttribute)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var parameterAttributes = parameter.GetCustomAttributes().ToArray();

            var parameterType = parameter.ParameterType;
            var parameterAlias = parameterAttributes.OfType<AliasAsAttribute>().FirstOrDefault();
            var parameterName = parameterAlias == null ? parameter.Name : parameterAlias.Name;
            var validationAttributes = parameterAttributes.OfType<ValidationAttribute>().ToReadOnlyList();

            this.Member = parameter;
            this.Name = parameterName ?? string.Empty;
            this.Index = parameter.Position;
            this.ParameterType = parameterType;
            this.ValidationAttributes = validationAttributes;

            var attributes = this.GetAttributes(parameter, parameterAttributes).ToArray();
            if (attributes.Length == 0)
            {
                this.Attributes = new[] { defaultAttribute }.ToReadOnlyList();
            }
            else
            {
                this.Attributes = attributes.ToReadOnlyList();
            }
        }

        /// <summary>
        /// 获取参数的特性
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="attributes">参数声明的所有特性</param> 
        /// <returns></returns>
        protected virtual IEnumerable<IApiParameterAttribute> GetAttributes(ParameterInfo parameter, Attribute[] attributes)
        {
            var parameterType = parameter.ParameterType;
            if (parameterType.IsInheritFrom<HttpContent>() == true)
            {
                return RepeatOne<HttpContentTypeAttribute>();
            }

            if (parameterType.IsInheritFrom<IApiParameter>() || parameterType.IsInheritFrom<IEnumerable<IApiParameter>>())
            {
                return RepeatOne<ApiParameterTypeAttribute>();
            }

            if (parameterType == typeof(CancellationToken) || parameterType.IsInheritFrom<IEnumerable<CancellationToken>>())
            {
                return RepeatOne<CancellationTokenTypeAttribute>();
            }

            if (parameterType == typeof(FileInfo) || parameterType.IsInheritFrom<IEnumerable<FileInfo>>())
            {
                return RepeatOne<FileInfoTypeAttribute>();
            }

            return attributes.OfType<IApiParameterAttribute>();
        }

        /// <summary>
        /// 返回单次的迭代器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static IEnumerable<T> RepeatOne<T>() where T : new()
        {
            return Enumerable.Repeat(new T(), 1);
        }
    }
}
