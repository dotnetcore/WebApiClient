using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using WebApiClientCore.Attributes;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示请求Api的返回描述
    /// </summary>
    [DebuggerDisplay("ReturnType = {ReturnType}")]
    public class DefaultApiReturnDescriptor : ApiReturnDescriptor
    {
        /// <summary>
        /// 获取返回类型
        /// </summary>
        public override Type ReturnType { get; protected set; }

        /// <summary>
        /// 获取ITask(Of T)或Task(Of T)的T类型描述
        /// </summary>
        public override ApiDataTypeDescriptor DataType { get; protected set; }

        /// <summary>
        /// 获取关联的IApiReturnAttribute
        /// </summary>
        public override IReadOnlyList<IApiReturnAttribute> Attributes { get; protected set; }

        /// <summary>
        /// 请求Api的返回描述
        /// for test only
        /// </summary>
        /// <param name="method">方法信息</param>  
        internal DefaultApiReturnDescriptor(MethodInfo method)
            : this(method, method.DeclaringType!)
        {
        }

        /// <summary>
        /// 请求Api的返回描述
        /// </summary>
        /// <param name="method">方法信息</param>
        /// <param name="interfaceType">接口类型</param> 
        public DefaultApiReturnDescriptor(MethodInfo method, Type interfaceType)
            : this(method.ReturnType, method.GetCustomAttributes(), interfaceType.GetInterfaceCustomAttributes())
        {
        }

        /// <summary>
        /// 请求Api的返回描述
        /// </summary>
        /// <param name="returnType">方法返回类型</param> 
        /// <param name="methodAttributes">方法的特性</param>
        /// <param name="interfaceAttributes">接口的特性</param> 
        public DefaultApiReturnDescriptor(Type returnType, IEnumerable<Attribute> methodAttributes, IEnumerable<Attribute> interfaceAttributes)
        {
            var type = returnType.IsGenericType
                ? returnType.GetGenericArguments().First()
                : typeof(HttpResponseMessage);

            var dataType = new DefaultApiDataTypeDescriptor(type);

            this.ReturnType = returnType;
            this.DataType = dataType;
            this.Attributes = methodAttributes
                .OfType<IApiReturnAttribute>()
                .Concat(interfaceAttributes.OfType<IApiReturnAttribute>())
                .Concat(GetDefaultAttributes(dataType))
                .Distinct(MultiplableComparer<IApiReturnAttribute>.Instance)
                .OrderBy(item => item.OrderIndex)
                .Where(item => item.Enable)
                // 最后步骤为比较媒体类型
                .Distinct(MediaTypeComparer.Default)
                .ToReadOnlyList();
        }

        /// <summary>
        /// 获取默认特性
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private static IEnumerable<IApiReturnAttribute> GetDefaultAttributes(ApiDataTypeDescriptor dataType)
        {
            const double acceptQuality = 0.1;
            if (dataType.IsRawType == true)
            {
                yield return new RawReturnAttribute(acceptQuality);
            }
            else
            {
                yield return new NoneReturnAttribute(acceptQuality);
                yield return new JsonReturnAttribute(acceptQuality) { EnsureMatchAcceptContentType = true };
                yield return new XmlReturnAttribute(acceptQuality) { EnsureMatchAcceptContentType = true };
            }
        }


        /// <summary>
        /// MediaType比较器
        /// </summary>
        private class MediaTypeComparer : IEqualityComparer<IApiReturnAttribute>
        {
            /// <summary>
            /// 获取默认实例
            /// </summary>
            public static MediaTypeComparer Default { get; } = new MediaTypeComparer();

            /// <summary>
            /// 是否相等
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(IApiReturnAttribute? x, IApiReturnAttribute? y)
            {
                if (x == null || y == null)
                {
                    return false;
                }

                var xVal = x.AcceptContentType?.MediaType;
                var yVal = y.AcceptContentType?.MediaType;

                // 允许多个无AcceptContentType的ApiReturnAttribute存在
                if (xVal == null && yVal == null)
                {
                    return false;
                }

                return string.Equals(xVal, yVal, StringComparison.OrdinalIgnoreCase);
            }

            /// <summary>
            /// 获取哈希码
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns> 
            public int GetHashCode(IApiReturnAttribute obj)
            {
                var mediaType = obj.AcceptContentType?.MediaType;
                if (mediaType == null)
                {
                    return 0;
                }
                return mediaType.GetHashCode(StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
