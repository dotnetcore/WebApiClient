namespace WebApiClientCore.Masking
{
    /// <summary>
    /// 表示字段值的脱敏策略
    /// </summary>
    public interface IMaskStrategy
    {
        /// <summary>
        /// 获取策略名称
        /// 用于在声明字段时指定策略
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 对字段值的文本执行脱敏
        /// </summary>
        /// <param name="value">
        /// 字段值的文本
        /// JSON 字符串值传入反转义后的内容，其它类型传入原始 JSON 文本
        /// </param>
        /// <returns>脱敏后的文本，返回原文本表示不需要脱敏</returns>
        string Mask(string value);
    }
}
