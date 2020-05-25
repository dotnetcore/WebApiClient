using System;
using System.Buffers;
using System.Diagnostics;

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
        [DebuggerDisplay("Count = {Count}")]
        [DebuggerTypeProxy(typeof(ArrayOwnerDebugView<>))]
        private class ArrayOwner<T> : IArrayOwner<T>
        {
            /// <summary>
            /// 获取持有的数组
            /// </summary>
            public T[] Array { get; }

            /// <summary>
            /// 获取数组的有效长度
            /// </summary>
            public int Count { get; }

            /// <summary>
            /// 数组持有者
            /// </summary>
            /// <param name="minLength"></param> 
            public ArrayOwner(int minLength)
            {
                this.Array = ArrayPool<T>.Shared.Rent(minLength);
                this.Count = minLength;
            }

            /// <summary>
            /// 归还数组
            /// </summary>
            public void Dispose()
            {
                ArrayPool<T>.Shared.Return(this.Array);
            }
        }

        /// <summary>
        /// 调试视图
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class ArrayOwnerDebugView<T>
        {
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public T[] Items { get; }

            /// <summary>
            /// 调试视图
            /// </summary>
            /// <param name="owner"></param>
            public ArrayOwnerDebugView(IArrayOwner<T> owner)
            {
                this.Items = owner.Array.AsSpan(0, owner.Count).ToArray();
            }
        }
    }
}
