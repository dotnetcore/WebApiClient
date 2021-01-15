using System;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义获取BufferWriter的数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IBufferWritten<T>
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
