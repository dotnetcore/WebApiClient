using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using WebApiClientCore.Attributes;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示请求Api的返回描述
    /// </summary>
    [DebuggerDisplay("ReturnType = {ReturnType}")]
    public class ApiReturnDescriptor
    {
        /// <summary>
        /// 获取返回类型
        /// </summary>
        public Type ReturnType { get; }

        /// <summary>
        /// 获取ITask(Of T)或Task(Of T)的T类型描述
        /// </summary>
        public ApiDataTypeDescriptor DataType { get; } 

        /// <summary>
        /// 获取关联的IApiReturnAttribute
        /// </summary>
        public IReadOnlyList<IApiReturnAttribute> Attributes { get; }

        /// <summary>
        /// 请求Api的返回描述
        /// </summary>
        /// <param name="method">方法信息</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiReturnDescriptor(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var type = method.ReturnType.IsGenericType ?
                method.ReturnType.GetGenericArguments().FirstOrDefault() :
                typeof(HttpResponseMessage);

            var dataType = new ApiDataTypeDescriptor(type);

            this.ReturnType = method.ReturnType;
            this.DataType = dataType;         
            this.Attributes = method
                .GetAttributes<IApiReturnAttribute>(true)
                .Concat(GetDefaultAttributes(dataType))
                .Distinct(new MultiplableComparer<IApiReturnAttribute>())
                .OrderBy(item => item.OrderIndex)
                .Where(item => item.Enable)
                .ToReadOnlyList();
        }

        /// <summary>
        /// 获取默认特性
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private static IEnumerable<IApiReturnAttribute> GetDefaultAttributes(ApiDataTypeDescriptor dataType)
        {
            const double acceptQuality = 0.01d;
            if (dataType.IsRawType == true)
            {
                yield return new RawReturnAttribute(acceptQuality);
                yield return new JsonReturnAttribute(acceptQuality);
                yield return new XmlReturnAttribute(acceptQuality);
            }
            else
            {
                yield return new JsonReturnAttribute(acceptQuality);
                yield return new XmlReturnAttribute(acceptQuality);
                yield return new RawReturnAttribute(acceptQuality);
            }
        }
    }
}
