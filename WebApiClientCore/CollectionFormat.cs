namespace WebApiClientCore
{
    /// <summary>
    /// 集合格式化方式
    /// </summary>
    public enum CollectionFormat : byte
    {
        /// <summary>
        /// 逗号分隔
        /// value1,value2
        /// </summary>
        Csv,

        /// <summary>
        /// 空格分隔
        /// value1 value2
        /// </summary>
        Ssv,

        /// <summary>
        /// 反斜杠分隔
        /// value1\value2
        /// </summary>
        Tsv,

        /// <summary>
        /// 竖线分隔
        /// value1|value2
        /// </summary>
        Pipes,

        /// <summary>
        /// 单属性可以取多个值
        /// </summary>
        Multi
    }
}
