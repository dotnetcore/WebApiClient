using System;
using System.Linq;
using System.Reflection;

namespace WebApiClient.Attributes.ReturnAttributes
{
    /// <summary>返回值转换声明属性</summary>
    public class ReturnValueMapperAttribute : Attribute
    {
        /// <summary>回复的类型</summary>
        public Type ResponseType { get; }

        /// <summary>回复内容转返回值转换器类型</summary>
        public Type ReturnValueMapperType { get; }

        /// <summary>ctor</summary>
        /// <param name="responseType">回复的类型，不能为空</param>
        /// <param name="returnValueMapperType">回复内容转返回值转换器类型，必须实现<see cref="IReturnValueMapper"/></param>
        public ReturnValueMapperAttribute(Type responseType, Type returnValueMapperType)
        {
            if (returnValueMapperType == null) throw new ArgumentNullException(nameof(returnValueMapperType));

            if (!typeof(IReturnValueMapper).IsAssignableFrom(returnValueMapperType))
                throw new ArgumentException("Not implement IReturnValueMapper ", nameof(returnValueMapperType));

            if (returnValueMapperType.GetTypeInfo().IsAbstract)
                throw new ArgumentException("Should not be abstract", nameof(returnValueMapperType));

            if (returnValueMapperType.GetConstructors().All(c => c.GetParameters().Length > 0))
                throw new ArgumentException("No parameterless constructor defined", nameof(returnValueMapperType));

            ResponseType = responseType ?? throw new ArgumentNullException(nameof(responseType));
            ReturnValueMapperType = returnValueMapperType;
        }
    }
}
