using System;

namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// 表示IApiParameter类型序列化异常
    /// </summary>
    public class ApiParameterSerializeException : ApiException
    {
        /// <summary>
        /// ApiParameter的类型
        /// </summary>
        public Type ApiParameterType { get; }

        /// <summary>
        /// IApiParameter类型序列化异常
        /// </summary>
        /// <param name="apiParameterType">ApiParameter的类型</param>
        public ApiParameterSerializeException(Type apiParameterType)
            : base(Resx.unsupported_SerializeApiParametern.Format(apiParameterType.Name))
        {
            this.ApiParameterType = apiParameterType;
        }
    }
}
