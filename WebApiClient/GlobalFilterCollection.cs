using System;
using System.Collections;
using System.Collections.Generic;
using WebApiClient;

namespace WebApiClient
{
    /// <summary>
    /// 表示全局过滤器的集合
    /// 全局过滤器执行优先级最高，执行顺序为添加的顺序
    /// 非线程安全类型
    /// </summary>
    public class GlobalFilterCollection : ICollection<IApiActionFilter>
    {
        /// <summary>
        /// 保存数据的列表
        /// </summary>
        private readonly List<IApiActionFilter> filters;

        /// <summary>
        /// 全局过滤器的集合
        /// </summary>
        public GlobalFilterCollection()
        {
            this.filters = new List<IApiActionFilter>();
        }

        /// <summary>
        /// 获取过滤器的数量
        /// </summary>
        public int Count
        {
            get
            {
                return this.filters.Count;
            }
        }

        /// <summary>
        /// 添加全局过滤器
        /// </summary>
        /// <param name="item">全局过滤器</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Add(IApiActionFilter item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            this.filters.Add(item);
        }

        /// <summary>
        /// 清除所有全局过滤器
        /// </summary>
        public void Clear()
        {
            this.filters.Clear();
        }

        /// <summary>
        /// 返回是否包含指定的全局过滤器
        /// </summary>
        /// <param name="item">全局过滤器</param>
        /// <returns></returns>
        public bool Contains(IApiActionFilter item)
        {
            if (item == null)
            {
                return false;
            }
            return this.filters.Contains(item);
        }

        /// <summary>
        /// 删除指定的全局过滤器
        /// </summary>
        /// <param name="item">定的全局过滤器</param>
        /// <returns></returns>
        public bool Remove(IApiActionFilter item)
        {
            if (item == null)
            {
                return false;
            }
            return this.filters.Remove(item);
        }

        /// <summary>
        /// 返回过滤器的迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IApiActionFilter> GetEnumerator()
        {
            return this.filters.GetEnumerator();
        }

        #region 显式实现
        /// <summary>
        /// 获取是否是只读的
        /// </summary>
        bool ICollection<IApiActionFilter>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 复制到目前数组
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        void ICollection<IApiActionFilter>.CopyTo(IApiActionFilter[] array, int arrayIndex)
        {
            this.filters.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.filters.GetEnumerator();
        }
        #endregion
    }
}
