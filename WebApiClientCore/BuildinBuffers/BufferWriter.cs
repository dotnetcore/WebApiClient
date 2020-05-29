using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示字节缓冲区写入对象
    /// </summary>
    class BufferWriter<T> : Disposable, IBufferWriter<T>
    {
        private const int MinimumBufferSize = 256;
        private IArrayOwner<T> byteArrayOwner;

        /// <summary>
        /// 获取已写入的字节数
        /// </summary>
        public int WrittenCount { get; private set; }

        /// <summary>
        /// 获取容量
        /// </summary>
        public int Capacity => this.byteArrayOwner.Array.Length;


        /// <summary>
        /// 字节缓冲区写入对象
        /// </summary>
        /// <param name="initialCapacity">初始容量</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public BufferWriter(int initialCapacity = 1024)
        {
            if (initialCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity));
            }
            this.byteArrayOwner = ArrayPool.Rent<T>(initialCapacity);
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void Clear()
        {
            this.byteArrayOwner.Array.AsSpan(0, this.WrittenCount).Clear();
            this.WrittenCount = 0;
        }

        /// <summary>
        /// 设置向前推进
        /// </summary>
        /// <param name="count"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Advance(int count)
        {
            if (count < 0 || this.WrittenCount + count > this.Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            this.WrittenCount += count;
        }

        /// <summary>
        /// 返回用于写入数据的Memory
        /// </summary>
        /// <param name="sizeHint">意图大小</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public Memory<T> GetMemory(int sizeHint = 0)
        {
            this.CheckAndResizeBuffer(sizeHint);
            return this.byteArrayOwner.Array.AsMemory(this.WrittenCount);
        }

        /// <summary>
        /// 返回用于写入数据的Span
        /// </summary>
        /// <param name="sizeHint">意图大小</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public Span<T> GetSpan(int sizeHint = 0)
        {
            this.CheckAndResizeBuffer(sizeHint);
            return byteArrayOwner.Array.AsSpan(this.WrittenCount);
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
        /// <returns></returns>
        public int Write(ReadOnlySpan<T> value)
        {
            var length = value.Length;
            if (length == 0)
            {
                return 0;
            }

            value.CopyTo(this.GetSpan(length));
            this.Advance(length);
            return length;
        }

        /// <summary>
        /// 获取已数入的数据
        /// </summary>
        public ReadOnlySpan<T> GetWrittenSpan()
        {
            return this.byteArrayOwner.Array.AsSpan(0, this.WrittenCount);
        }

        /// <summary>
        /// 获取已数入的数据
        /// </summary>
        public ReadOnlyMemory<T> GetWrittenMemory()
        {
            return this.byteArrayOwner.Array.AsMemory(0, this.WrittenCount);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.byteArrayOwner?.Dispose();
        }

        /// <summary>
        /// 检测和扩容
        /// </summary>
        /// <param name="sizeHint"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckAndResizeBuffer(int sizeHint)
        {
            if (sizeHint < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeHint));
            }

            if (sizeHint == 0)
            {
                sizeHint = MinimumBufferSize;
            }

            var freeCapacity = this.Capacity - this.WrittenCount;
            if (sizeHint > freeCapacity)
            {
                var growBy = Math.Max(sizeHint, this.Capacity);
                var newSize = checked(this.Capacity + growBy);

                var newOwer = ArrayPool.Rent<T>(newSize);
                this.GetWrittenSpan().CopyTo(newOwer.Array);
                this.byteArrayOwner.Dispose();
                this.byteArrayOwner = newOwer;
            }
        }
    }
}