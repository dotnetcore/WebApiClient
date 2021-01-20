using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示请求Api的参数描述
    /// </summary>
    public abstract class ApiParameterDescriptor
    {
        /// <summary>
        /// 获取参数名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 获取关联的参数信息
        /// </summary>
        public abstract ParameterInfo Member { get; }

        /// <summary>
        /// 获取参数索引
        /// </summary>
        public abstract int Index { get; }

        /// <summary>
        /// 获取参数类型
        /// </summary>
        public abstract Type ParameterType { get; }

        /// <summary>
        /// 获取关联的参数特性
        /// </summary>
        public abstract IReadOnlyList<IApiParameterAttribute> Attributes { get; }

        /// <summary>
        /// 获取关联的ValidationAttribute特性
        /// </summary>
        public abstract IReadOnlyList<ValidationAttribute> ValidationAttributes { get; }
    }
}
