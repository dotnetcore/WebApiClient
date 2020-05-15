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
        public Type Type { get; protected set; }

        /// <summary>
        /// 获取是否为String类型
        /// </summary>
        public bool IsString { get; protected set; }

        /// <summary>
        /// 获取是否为Stream类型
        /// </summary>
        public bool IsStream { get; protected set; }

        /// <summary>
        /// 获取是否为byte[]类型
        /// </summary>
        public bool IsByteArray { get; protected set; }

        /// <summary>
        ///  获取是否为HttpResponseMessage类型
        /// </summary>
        public bool IsHttpResponseMessage { get; protected set; }

        /// <summary>
        /// 获取是否为自定义类型
        /// </summary>
        public bool IsCustomType { get; protected set; }

        /// <summary>
        /// 返回的Task(Of T)的T类型描述
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiDataTypeDescriptor(Type dataType)
        {
            this.Type = dataType ?? throw new ArgumentNullException(nameof(dataType));

            this.IsString = dataType == typeof(string);
            this.IsStream = dataType == typeof(Stream);
            this.IsByteArray = dataType == typeof(byte[]);
            this.IsHttpResponseMessage = dataType == typeof(HttpResponseMessage);
            this.IsCustomType = !(IsString || IsStream || IsByteArray || IsHttpResponseMessage);
        }
    }
}
