using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace WebApiClient.Contexts
{
    /// <summary>
    /// 表示返回的Task(Of T)的T类型描述
    /// </summary>
    [DebuggerDisplay("Type = {Type}")]
    public class DataTypeDescriptor
    {
        /// <summary>
        /// 获取类型
        /// </summary>
        public Type Type { get; protected set; }

        /// <summary>
        /// 获取包装为ITask的创建工厂
        /// </summary>
        public Func<ITask> ITaskFactory { get; protected set; }

        /// <summary>
        /// 获取HttpResponseWrapper子类的创建工厂
        /// </summary>
        public Func<HttpResponseMessage, HttpResponseWrapper> HttpResponseWrapperFactory { get; protected set; }

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
        /// 获取是否为HttpResponseWrapper子类型
        /// </summary>
        public bool IsHttpResponseWrapper { get; protected set; }

        /// <summary>
        /// 返回的Task(Of T)的T类型描述
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DataTypeDescriptor(Type dataType)
        {
            this.Type = dataType ?? throw new ArgumentNullException(nameof(dataType));

            this.IsString = dataType == typeof(string);
            this.IsStream = dataType == typeof(Stream);
            this.IsByteArray = dataType == typeof(byte[]);
            this.IsHttpResponseMessage = dataType == typeof(HttpResponseMessage);
            this.IsHttpResponseWrapper = dataType.IsInheritFrom<HttpResponseWrapper>();

            var apiTaskType = typeof(ApiTask<>).MakeGenericType(dataType);
            this.ITaskFactory = Lambda.CreateCtorFunc<ITask>(apiTaskType);

            if (this.IsHttpResponseWrapper == true)
            {
                this.HttpResponseWrapperFactory = Lambda.CreateCtorFunc<HttpResponseMessage, HttpResponseWrapper>(dataType);
            }
        }
    }
}
