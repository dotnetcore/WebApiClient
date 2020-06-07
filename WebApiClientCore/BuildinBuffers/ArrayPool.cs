using System.Buffers;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示共享的数组池
    /// </summary>
    static class ArrayPool
    {
        /// <summary>
        /// 租赁数组
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="minLength">最小长度</param>
        /// <returns></returns>
        public static IArrayOwner<T> Rent<T>(int minLength)
        {
            return new ArrayOwner<T>(minLength);
        }

        /// <summary>
        /// 表示数组持有者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private sealed class ArrayOwner<T> : Disposable, IArrayOwner<T>
        {
            /// <summary>
            /// 获取持有的数组
            /// </summary>
            public T[] Array { get; }

            /// <summary>
            /// 数组持有者
            /// </summary>
            /// <param name="minLength"></param> 
            public ArrayOwner(int minLength)
            {
                this.Array = ArrayPool<T>.Shared.Rent(minLength);
            }

            /// <summary>
            /// 归还数组
            /// </summary>
            /// <param name="disposing"></param>
            protected sealed override void Dispose(bool disposing)
            {
                ArrayPool<T>.Shared.Return(this.Array);
            }
        }
    }
}
