using System;
using System.Linq;

namespace WebApiClient
{
    /// <summary>
    /// 表示Guid16位
    /// </summary>
    struct Guid16 : IComparable<Guid16>, IEquatable<Guid16>
    {
        /// <summary>
        /// 值
        /// </summary>
        private readonly long val;

        /// <summary>
        /// 表示空的Guid16
        /// </summary>
        public static readonly Guid16 Empty = new Guid16(0);

        /// <summary>
        /// Guid16位
        /// </summary>
        /// <param name="val">值</param>
        public Guid16(long val)
        {
            this.val = val;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.val.ToString("x16");
        }

        /// <summary>
        /// 转换为64位整数
        /// </summary>
        /// <returns></returns>
        public long ToInt64()
        {
            return this.val;
        }

        /// <summary>
        /// 返回哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.val.GetHashCode();
        }

        /// <summary>
        /// 返回是否与目标相等
        /// </summary>
        /// <param name="obj">目标</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.val.Equals(obj);
        }

        /// <summary>
        /// 返回是否与目标相等
        /// </summary>
        /// <param name="other">目标</param>
        /// <returns></returns>
        public bool Equals(Guid16 other)
        {
            return this.val.Equals(other.val);
        }

        /// <summary>
        /// 和目标比较大小
        /// </summary>
        /// <param name="other">目标</param>
        /// <returns></returns>
        public int CompareTo(Guid16 other)
        {
            return this.val.CompareTo(other.val);
        }

        /// <summary>
        /// 创建新的Guid16
        /// </summary>
        /// <returns></returns>
        public static Guid16 NewGuid16()
        {
            var val = Guid.NewGuid().ToByteArray().Aggregate<byte, long>(1, (current, b) => current * (b + 1));
            return new Guid16(val - DateTime.Now.Ticks);
        }

        /// <summary>
        /// 转换为Guid16
        /// </summary>
        /// <param name="g">16位hex</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static Guid16 Parse(string g)
        {
            if (string.IsNullOrEmpty(g) == true)
            {
                throw new ArgumentNullException(nameof(g));
            }

            if (g.Length != 16)
            {
                throw new ArgumentException(nameof(g));
            }

            var val = Convert.ToInt64(g, 16);
            return new Guid16(val);
        }

        /// <summary>
        /// 返回是否相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Guid16 a, Guid16 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// 返回不相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Guid16 a, Guid16 b)
        {
            return a.Equals(b) == false;
        }
    }
}
