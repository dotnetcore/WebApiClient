namespace WebApiClientCore.Masking
{
    /// <summary>
    /// 表示脱敏的配置选项
    /// </summary>
    public sealed class MaskingOptions
    {
        /// <summary>
        /// 获取或设置脱敏策略的注册表
        /// </summary>
        public MaskStrategyRegistry Strategies { get; set; } = new MaskStrategyRegistry();
    }
}
