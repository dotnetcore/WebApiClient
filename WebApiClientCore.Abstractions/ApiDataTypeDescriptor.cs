using System;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示返回的Task(Of T)的T类型描述
    /// </summary> 
    public abstract class ApiDataTypeDescriptor
    {
        /// <summary>
        /// 获取类型
        /// </summary>
        public abstract Type Type { get; protected set; }

        /// <summary>
        /// 获取是否为原始类型的String
        /// </summary>
        public abstract bool IsRawString { get; protected set; }

        /// <summary>
        /// 获取是否为原始类型的Stream
        /// </summary>
        public abstract bool IsRawStream { get; protected set; }

        /// <summary>
        /// 获取是否为原始类型的byte[]
        /// </summary>
        public abstract bool IsRawByteArray { get; protected set; }

        /// <summary>
        ///  获取是否为原始类型的HttpResponseMessage
        /// </summary>
        public abstract bool IsRawHttpResponseMessage { get; protected set; }

        /// <summary>
        /// 获取是否为原始类型中的一个
        /// </summary>
        public abstract bool IsRawType { get; protected set; }
    }
}
