using System;
using System.Buffers;
using System.Diagnostics;

namespace WebApiClientCore
{
    /// <summary>
    /// 将Memory包装为BufferWriter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("WrittenCount = {WrittenCount}")]
    sealed class MemoryBufferWriter<T> : IBufferWriter<T>
    {
        private int index;
        private Memory<T> memory;

        /// <summary>
        /// 获取已数入的数据长度
        /// </summary>
        public int WrittenCount => this.index;

        /// <summary>
        /// 获取剩余容量
        /// </summary>
        public int FreeCapacity => this.memory.Length;

        /// <summary>
        /// Memory包装为BufferWriter
        /// </summary>
        /// <param name="memory"></param>
        public MemoryBufferWriter(Memory<T> memory)
        {
            this.index = 0;
            this.memory = memory;
        }

        /// <summary>
        /// 设置向前推进
        /// </summary>
        /// <param name="count"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Advance(int count)
        {
            if (count < 0 || count > this.memory.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            this.memory = this.memory.Slice(count);
            this.index += count;
        }

        /// <summary>
        /// 返回用于写入数据的Span
        /// </summary>
        /// <param name="sizeHint">意图大小</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public Span<T> GetSpan(int sizeHint = 0)
        {
            return this.GetMemory(sizeHint).Span;
        }

        /// <summary>
        /// 返回用于写入数据的Memory
        /// </summary>
        /// <param name="sizeHint">意图大小</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public Memory<T> GetMemory(int sizeHint = 0)
        {
            if (sizeHint > this.memory.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeHint));
            }
            return this.memory;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="value"></param>
        public void Write(T value)
        {
            this.GetSpan(1)[0] = value;
            this.Advance(1);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="value">值</param> 
        public void Write(ReadOnlySpan<T> value)
        {
            if (value.IsEmpty == false)
            {
                value.CopyTo(this.GetSpan(value.Length));
                this.Advance(value.Length);
            }
        }
    }
}
