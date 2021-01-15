using System;
using System.Buffers;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示自动扩容的BufferWriter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    sealed class ResizableBufferWriter<T> : IBufferWriter<T>
    {
        private const int maxArrayLength = 0X7FEFFFFF;
        private const int defaultSizeHint = 256;

        private int index;
        private T[] buffer;

        /// <summary>
        /// 自动扩容的BufferWriter
        /// </summary>
        /// <param name="initialCapacity">初始容量</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ResizableBufferWriter(int initialCapacity)
        {
            if (initialCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity));
            }

            this.buffer = new T[initialCapacity];
            this.index = 0;
        }

        /// <summary>
        /// 获取已数入的数据
        /// </summary>
        public ReadOnlyMemory<T> WrittenMemory => this.buffer.AsMemory(0, index);

        /// <summary>
        /// 获取已数入的数据
        /// </summary>
        public ReadOnlySpan<T> WrittenSpan => this.buffer.AsSpan(0, index);

        /// <summary>
        /// 获取已数入的数据
        /// </summary>
        public int WrittenCount => this.index;

        /// <summary>
        /// 获取容量
        /// </summary>
        public int Capacity => this.buffer.Length;

        /// <summary>
        /// 获取剩余容量
        /// </summary>
        public int FreeCapacity => this.buffer.Length - this.index;

        /// <summary>
        /// 清除数据
        /// </summary>
        public void Clear()
        {
            this.buffer.AsSpan(0, this.index).Clear();
            this.index = 0;
        }

        /// <summary>
        /// 设置向前推进
        /// </summary>
        /// <param name="count"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Advance(int count)
        {
            if (count < 0 || this.index > this.buffer.Length - count)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            this.index += count;
        }


        /// <summary>
        /// 返回用于写入数据的Memory
        /// </summary>
        /// <param name="sizeHint">意图大小</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public Memory<T> GetMemory(int sizeHint = 0)
        {
            CheckAndResizeBuffer(sizeHint);
            return this.buffer.AsMemory(this.index);
        }


        /// <summary>
        /// 返回用于写入数据的Span
        /// </summary>
        /// <param name="sizeHint">意图大小</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public Span<T> GetSpan(int sizeHint = 0)
        {
            CheckAndResizeBuffer(sizeHint);
            return this.buffer.AsSpan(this.index);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="value"></param>
        public void Write(T value)
        {
            this.GetSpan(1)[0] = value;
            this.index += 1;
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
                this.index += value.Length;
            }
        }

        /// <summary>
        /// 检测和扩容
        /// </summary>
        /// <param name="sizeHint"></param>
        private void CheckAndResizeBuffer(int sizeHint)
        {
            if (sizeHint < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeHint));
            }

            if (sizeHint == 0)
            {
                sizeHint = 1;
            }

            if (sizeHint > this.FreeCapacity)
            {
                var currentLength = this.buffer.Length;
                var growBy = Math.Max(sizeHint, currentLength);

                if (currentLength == 0)
                {
                    growBy = Math.Max(growBy, defaultSizeHint);
                }

                var newSize = currentLength + growBy;
                if ((uint)newSize > int.MaxValue)
                {
                    var needed = (uint)(currentLength - this.FreeCapacity + sizeHint);
                    if (needed > maxArrayLength)
                    {
                        throw new OutOfMemoryException();
                    }

                    newSize = maxArrayLength;
                }

                Array.Resize(ref this.buffer, newSize);
            }
        }
    }
}
