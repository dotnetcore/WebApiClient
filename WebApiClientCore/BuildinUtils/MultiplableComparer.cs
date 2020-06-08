using System.Collections.Generic;

namespace WebApiClientCore
{
    /// <summary>
    /// 是否允许重复的特性比较器
    /// </summary>
    sealed class MultiplableComparer<TAttributeMultiplable> : IEqualityComparer<TAttributeMultiplable> where TAttributeMultiplable : IAttributeMultiplable
    {
        /// <summary>
        /// 获取默认实例
        /// </summary>
        public static MultiplableComparer<TAttributeMultiplable> Default { get; } = new MultiplableComparer<TAttributeMultiplable>();

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(TAttributeMultiplable x, TAttributeMultiplable y)
        {
            // 如果其中一个不允许重复，返回true将y过滤
            return x.AllowMultiple == false || y.AllowMultiple == false;
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns> 
        public int GetHashCode(TAttributeMultiplable obj)
        {
            return obj.GetType().GetHashCode();
        }
    }
}
