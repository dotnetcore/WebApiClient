using System;
using System.Text.RegularExpressions;

namespace WebApiClientCore.Masking.Strategies
{
    /// <summary>
    /// 表示身份证号的脱敏策略
    /// 110101199001011234 脱敏为 110101********1234
    /// </summary>
    public sealed class IdCardMaskStrategy : IMaskStrategy
    {
        /// <summary>
        /// 获取内置策略的名称
        /// </summary>
        public const string StrategyName = "IdCard";

        private static readonly Regex idCard18Regex = new(
            pattern: @"^(\d{6})\d{8}(\d{4})$",
            options: RegexOptions.CultureInvariant,
            matchTimeout: TimeSpan.FromSeconds(1));

        private static readonly Regex idCard15Regex = new(
            pattern: @"^(\d{6})\d{5}(\d{4})$",
            options: RegexOptions.CultureInvariant,
            matchTimeout: TimeSpan.FromSeconds(1));

        /// <summary>
        /// 获取策略名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 身份证号的脱敏策略
        /// </summary>
        /// <param name="name">策略名称，默认为 IdCard</param>
        public IdCardMaskStrategy(string? name = null)
        {
            this.Name = string.IsNullOrWhiteSpace(name) == true ? StrategyName : name!;
        }

        /// <summary>
        /// 保留前 6 位和后 4 位，不符合格式时返回原值
        /// </summary>
        /// <param name="value">字段值</param>
        /// <returns></returns>
        public string Mask(string value)
        {
            if (idCard18Regex.IsMatch(value) == true)
            {
                return idCard18Regex.Replace(value, "$1********$2");
            }

            return idCard15Regex.IsMatch(value) == true
                ? idCard15Regex.Replace(value, "$1*****$2")
                : value;
        }
    }
}
