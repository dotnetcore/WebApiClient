using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using WebApiClientCore.Attributes;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 当非GET或HEAD请求的缺省参数特性声明时
    /// 为复杂参数类型的参数应用JsonContentAttribute的ApiActionDescriptor
    /// </summary>
    public class JsonFirstApiActionDescriptor : DefaultApiActionDescriptor
    {
        private static readonly IApiParameterAttribute pathQueryAttribute = new PathQueryAttribute();
        private static readonly IApiParameterAttribute jsonContentAttribute = new JsonContentAttribute();

        /// <summary>
        /// 当非GET或HEAD请求的缺省参数特性声明时
        /// 为复杂参数类型的参数应用JsonContentAttribute
        /// </summary>
        /// <param name="method"></param>
        /// <param name="interfaceType"></param>
        public JsonFirstApiActionDescriptor(MethodInfo method, Type interfaceType)
            : base(method, interfaceType)
        {
            var defineGetHead = this.Attributes.Any(a => this.IsGetHeadAttribute(a));
            if (defineGetHead == false)
            {
                this.Parameters = this.GetApiParameterDescriptors().ToReadOnlyList();
            }
        }

        /// <summary>
        /// 获取参数描述
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ApiParameterDescriptor> GetApiParameterDescriptors()
        {
            foreach (var parameter in this.Parameters)
            {
                var parameterType = parameter.ParameterType;
                var realType = Nullable.GetUnderlyingType(parameterType) ?? parameterType;
                var defaultAttribute = this.IsSimpleType(realType) ? pathQueryAttribute : jsonContentAttribute;
                yield return new DefaultApiParameterDescriptor(parameter.Member, defaultAttribute);
            }
        }

        /// <summary>
        /// 是否为Get或Head特性
        /// </summary>
        /// <param name="apiActionAttribute">方法特性</param>
        /// <returns></returns>
        protected virtual bool IsGetHeadAttribute(IApiActionAttribute apiActionAttribute)
        {
            if (apiActionAttribute is HttpMethodAttribute methodAttribute)
            {
                var httpMethod = methodAttribute.Method;
                return httpMethod == HttpMethod.Get || httpMethod == HttpMethod.Head;
            }
            return false;
        }

        /// <summary>
        /// 是否为简单类型
        /// 这些类型缺省特性时仍然使用PathQueryAttribute
        /// </summary>
        /// <param name="realType">真实类型，非nullable</param>
        /// <returns></returns>
        protected virtual bool IsSimpleType(Type realType)
        {
            return realType.IsPrimitive
                || realType.IsInheritFrom<Enum>()
                || realType == typeof(string)
                || realType == typeof(decimal)
                || realType == typeof(DateTime)
                || realType == typeof(DateTimeOffset)
                || realType == typeof(Guid)
                || realType == typeof(Uri)
                || realType == typeof(Version)
                || realType == typeof(TimeSpan)
                || realType == typeof(IPAddress);
        }
    }
}
