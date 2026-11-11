using System;
using System.Text.RegularExpressions;

namespace WebApiClientCore.Masking.Strategies
{
    /// <summary>
    /// 表示邮箱的脱敏策略
    /// zhangsan@example.com 脱敏为 z***@example.com
    /// </summary>
    public sealed class EmailMaskStrategy : IMaskStrategy
    {
        /// <summary>
        /// 获取内置策略的名称
        /// </summary>
        public const string StrategyName = "Email";

        private static readonly Regex emailRegex = new(
            pattern: @"^([^@\s])([^@\s]*)@(.+)$",
            options: RegexOptions.CultureInvariant,
            matchTimeout: TimeSpan.FromSeconds(1));

        /// <summary>
        /// 获取策略名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 邮箱的脱敏策略
        /// </summary>
        /// <param name="name">策略名称，默认为 Email</param>
        public EmailMaskStrategy(string? name = null)
        {
            this.Name = string.IsNullOrWhiteSpace(name) == true ? StrategyName : name!;
        }

        /// <summary>
        /// 保留邮箱的首字符和域名，不符合格式时返回原值
        /// </summary>
        /// <param name="value">字段值</param>
        /// <returns></returns>
        public string Mask(string value)
        {
            return emailRegex.IsMatch(value) == true
                ? emailRegex.Replace(value, "$1***@$3")
                : value;
        }
    }
}
