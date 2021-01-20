using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示返回的Task(Of T)的T类型描述
    /// </summary>
    [DebuggerDisplay("Type = {Type.Name}")]
    public class DefaultApiDataTypeDescriptor : ApiDataTypeDescriptor
    {
        /// <summary>
        /// 获取类型
        /// </summary>
        public override Type Type { get; protected set; }

        /// <summary>
        /// 获取是否为原始类型的String
        /// </summary>
        public override bool IsRawString { get; protected set; }

        /// <summary>
        /// 获取是否为原始类型的Stream
        /// </summary>
        public override bool IsRawStream { get; protected set; }

        /// <summary>
        /// 获取是否为原始类型的byte[]
        /// </summary>
        public override bool IsRawByteArray { get; protected set; }

        /// <summary>
        ///  获取是否为原始类型的HttpResponseMessage
        /// </summary>
        public override bool IsRawHttpResponseMessage { get; protected set; }

        /// <summary>
        /// 获取是否为原始类型中的一个
        /// </summary>
        public override bool IsRawType { get; protected set; }

        /// <summary>
        /// 返回的Task(Of T)的T类型描述
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DefaultApiDataTypeDescriptor(Type dataType)
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
