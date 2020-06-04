using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示返回的Task(Of T)的T类型描述
    /// </summary>
    [DebuggerDisplay("Type = {Type.Name}")]
    public class ApiDataTypeDescriptor
    {
        /// <summary>
        /// 获取类型
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 获取是否为原始类型的String
        /// </summary>
        public bool IsRawString { get; }

        /// <summary>
        /// 获取是否为原始类型的Stream
        /// </summary>
        public bool IsRawStream { get; }

        /// <summary>
        /// 获取是否为原始类型的byte[]
        /// </summary>
        public bool IsRawByteArray { get; }

        /// <summary>
        ///  获取是否为原始类型的HttpResponseMessage
        /// </summary>
        public bool IsRawHttpResponseMessage { get; }

        /// <summary>
        /// 获取是否为原始类型中的一个
        /// </summary>
        public bool IsRawType { get; }

        /// <summary>
        /// 返回的Task(Of T)的T类型描述
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiDataTypeDescriptor(Type dataType)
        {
            this.Type = dataType ?? throw new ArgumentNullException(nameof(dataType));

            this.IsRawString = dataType == typeof(string);
            this.IsRawStream = dataType == typeof(Stream);
            this.IsRawByteArray = dataType == typeof(byte[]);
            this.IsRawHttpResponseMessage = dataType == typeof(HttpResponseMessage);
            this.IsRawType = IsRawString || IsRawStream || IsRawByteArray || IsRawHttpResponseMessage;
        }
    }
}
