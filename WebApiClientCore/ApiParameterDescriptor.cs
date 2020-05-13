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

namespace WebApiClientCore
{
    /// <summary>
    /// 表示请求Api的参数描述
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
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

            this.Member = parameter;
            this.Name = parameterName;
            this.Index = parameter.Position;
            this.Attributes = attributes;
            this.ParameterType = parameterType;
            this.ValidationAttributes = validationAttributes;
        }

        /// <summary>
        /// 返回参数特性
        /// </summary>
        /// <param name="parameterType">参数类型</param>
        /// <param name="defined">参数上声明的特性</param>
        /// <returns></returns>
        protected virtual IEnumerable<IApiParameterAttribute> GetAttributes(Type parameterType, IEnumerable<IApiParameterAttribute> defined)
        {
            if (parameterType.IsInheritFrom<HttpContent>() == true)
            {
                return RepeatOne<HttpContentAttribute>();
            }

            if (parameterType.IsInheritFrom<IApiParameterable>() || parameterType.IsInheritFrom<IEnumerable<IApiParameterable>>())
            {
                return RepeatOne<ParameterableAttribute>();
            }

            if (parameterType == typeof(CancellationToken))
            {
                return RepeatOne<CancellationTokenAttribute>();
            }

            if (parameterType == typeof(FileInfo))
            {
                return RepeatOne<FileInfoAttribute>();
            }

            if (defined.Any() == true)
            {
                return defined;
            }

            return RepeatOne<PathQueryAttribute>();
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
