using System;
using System.Runtime.CompilerServices;

namespace WebApiClientCore.Internals
{
    /// <summary>
    /// StringBuilder
    /// </summary>
    public ref struct ValueStringBuilder
    {
        private int index;
        private char[]? array;
        private Span<char> chars;

        /// <summary>
        /// StringBuilder
        /// </summary>
        /// <param name="buffer"></param>
        public ValueStringBuilder(Span<char> buffer)
        {
            this.index = 0;
            this.array = null;
            this.chars = buffer;
        }

        /// <summary>
        /// 添加char
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ValueStringBuilder Append(char value)
        {
            var newSize = this.index + 1;
            if (newSize > this.chars.Length)
            {
                this.Grow(newSize);
            }

            this.chars[this.index..][0] = value;
            this.index = newSize;
            return this;
        }

        /// <summary>
        /// 添加chars
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ValueStringBuilder Append(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
            {
                return this;
            }

            var newSize = this.index + value.Length;
            if (newSize > this.chars.Length)
            {
                this.Grow(newSize);
            }

            value.CopyTo(this.chars[this.index..]);
            this.index = newSize;
            return this;
        }

        /// <summary>
        /// 扩容
        /// </summary>
        /// <param name="newSize"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Grow(int newSize)
        {
            var size = Math.Max(newSize, this.chars.Length * 2);
            if (this.array == null)
            {
                this.array = new char[size];
                this.chars.CopyTo(array);
            }
            else
            {
                Array.Resize(ref this.array, size);
            }
            this.chars = this.array.AsSpan();
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override readonly string ToString()
        {
            return this.chars.Slice(0, this.index).ToString();
        }
    }
}
