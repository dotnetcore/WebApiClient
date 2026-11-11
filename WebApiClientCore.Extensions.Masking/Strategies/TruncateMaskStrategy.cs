using System;

namespace WebApiClientCore.Masking.Strategies
{
    /// <summary>
    /// 表示保留前后若干位，中间用掩码替换的脱敏策略
    /// </summary>
    public sealed class TruncateMaskStrategy : IMaskStrategy
    {
        /// <summary>
        /// 获取内置策略的名称
        /// </summary>
        public const string StrategyName = "Truncate";

        /// <summary>
        /// 获取保留的起始位数
        /// </summary>
        public int KeepStart { get; }

        /// <summary>
        /// 获取保留的末尾位数
        /// </summary>
        public int KeepEnd { get; }

        /// <summary>
        /// 获取中间部分的掩码
        /// </summary>
        public string MaskText { get; }

        /// <summary>
        /// 获取策略名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 保留前后若干位，中间用掩码替换的脱敏策略
        /// </summary>
        /// <param name="keepStart">保留的起始位数</param>
        /// <param name="keepEnd">保留的末尾位数</param>
        /// <param name="maskText">中间部分的掩码</param>
        /// <param name="name">策略名称，默认为 Truncate</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public TruncateMaskStrategy(int keepStart = 3, int keepEnd = 4, string maskText = "***", string? name = null)
        {
            if (keepStart < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(keepStart));
            }

            if (keepEnd < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(keepEnd));
            }

            this.KeepStart = keepStart;
            this.KeepEnd = keepEnd;
            this.MaskText = maskText ?? throw new ArgumentNullException(nameof(maskText));
            this.Name = string.IsNullOrWhiteSpace(name) == true ? StrategyName : name!;
        }

        /// <summary>
        /// 保留前后若干位，中间用掩码替换
        /// </summary>
        /// <param name="value">字段值</param>
        /// <returns></returns>
        public string Mask(string value)
        {
            if (value.Length <= this.KeepStart + this.KeepEnd)
            {
                return value;
            }

            var head = value.Substring(0, this.KeepStart);
            var tail = value.Substring(value.Length - this.KeepEnd);
            return $"{head}{this.MaskText}{tail}";
        }
    }
}
