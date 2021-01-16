using System;
using System.Buffers;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义支持获取已写入数据的BufferWriter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IWrittenBufferWriter<T> : IBufferWriter<T>
    {
        /// <summary>
        /// 获取已数入的数据长度
        /// </summary>
        int WrittenCount { get; }

        /// <summary>
        /// 获取已数入的数据
        /// </summary>
        ReadOnlySpan<T> WrittenSpan { get; }

        /// <summary>
        /// 获取已数入的数据
        /// </summary>
        ReadOnlyMemory<T> WrittenMemory { get; }

        /// <summary>
        /// 获取已数入的数据
        /// </summary>
        /// <returns></returns>
        ArraySegment<T> WrittenSegment { get; } 
    }
}
